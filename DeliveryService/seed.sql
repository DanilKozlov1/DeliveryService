-- Этот файл выполняется ПОСЛЕ миграций EF Core
-- Запусти вручную один раз через pgAdmin или psql
 
-- =============================================
-- Seed: Категории
-- =============================================
INSERT INTO "Categories" ("Name") VALUES
    ('Все'),
    ('Напитки'),
    ('Десерты'),
    ('Горячее')
ON CONFLICT DO NOTHING;
 
-- =============================================
-- Seed: Еда
-- =============================================
INSERT INTO "Foods" ("Title", "Description", "ImageUrl", "Weight", "CategoriesId", "Price") VALUES
    ('Кофе',     'Ароматный американо',        'pack://application:,,,/Images/coffee.png', 200, 2, 150.00),
    ('Чизкейк',  'Нежный сливочный чизкейк',   'pack://application:,,,/Images/cheesecake.png', 150, 3, 350.00),
    ('Круассан', 'Свежий слоёный круассан',    'pack://application:,,,/Images/croissant.png', 100, 3, 180.00),
    ('Плов',     'Узбекский плов с бараниной',  'pack://application:,,,/Images/plov.png', 400, 4, 450.00),
    ('Борщ',     'Борщ со сметаной',            'https://images.unsplash.com/photo-1547592180-85f173990554?w=400', 400, 4, 320.00),
    ('Латте',    'Кофе латте с молоком',        'https://images.unsplash.com/photo-1561047029-3000c68339ca?w=400', 250, 2, 200.00)
ON CONFLICT DO NOTHING;
 
-- =============================================
-- Seed: Admin пользователь
-- =============================================
INSERT INTO "Clients" ("Name", "Phone", "Email", "Password", "Role", "Created_At") VALUES
    ('admin', '00000000000', 'admin@delivery.ru', 'admin', 'admin', NOW())
ON CONFLICT DO NOTHING;
 