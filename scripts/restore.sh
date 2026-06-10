#!/bin/bash

# Загрузка переменных из .env
set -a
source .env
set +a

DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-5433}
DB_USER=${DB_USER:-postgres}
DB_PASSWORD=${DB_PASSWORD:-admin}
DB_NAME=${DB_NAME:-delivery_db}

# Проверка аргумента
if [ -z "$1" ]; then
    echo "Использование: $0 <путь_к_папке_бэкапа>"
    echo "Пример: $0 ./backups/backup_20250610_120000"
    exit 1
fi

BACKUP_DIR="$1"
DB_DUMP_FILE=$(ls "$BACKUP_DIR"/db_dump_*.sql 2>/dev/null | head -n 1)
UPLOADS_ARCHIVE=$(ls "$BACKUP_DIR"/uploads_backup_*.tar.gz 2>/dev/null | head -n 1)

if [ -z "$DB_DUMP_FILE" ]; then
    echo "ОШИБКА: не найден файл дампа БД в $BACKUP_DIR" >&2
    exit 1
fi

echo "=== Восстановление системы из бэкапа $BACKUP_DIR ==="
echo "Дамп БД: $DB_DUMP_FILE"
if [ -n "$UPLOADS_ARCHIVE" ]; then
    echo "Архив медиа: $UPLOADS_ARCHIVE"
fi

# 1. Очистка текущей базы данных (удаление всех таблиц)
echo "Очистка текущей базы данных..."
export PGPASSWORD="$DB_PASSWORD"
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "DROP SCHEMA public CASCADE; CREATE SCHEMA public;" 2>/dev/null
if [ $? -ne 0 ]; then
    echo "Предупреждение: не удалось очистить схему. Возможно, база ещё не создана."
fi

# 2. Восстановление дампа
echo "Восстановление дампа базы данных..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$DB_DUMP_FILE"
if [ $? -eq 0 ]; then
    echo "Дамп успешно восстановлен."
else
    echo "ОШИБКА при восстановлении дампа" >&2
    unset PGPASSWORD
    exit 1
fi
unset PGPASSWORD

# 3. Восстановление медиа-файлов
if [ -n "$UPLOADS_ARCHIVE" ]; then
    echo "Восстановление медиа-файлов..."
    rm -rf ./uploads
    tar -xzf "$UPLOADS_ARCHIVE" -C .
    echo "Медиа-файлы восстановлены."
else
    echo "Архив медиа не найден, пропуск."
fi

echo "=== Восстановление завершено ==="
echo "Проверьте работоспособность приложения."