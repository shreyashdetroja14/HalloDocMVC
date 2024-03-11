CREATE TABLE order_details(
	order_id INTEGER PRIMARY KEY,
	customer_name VARCHAR(50) NOT NULL,
	product_name VARCHAR(50) NOT NULL,
	ordered_from VARCHAR(50) NOT NULL,
	ordered_amount NUMERIC(7,2),
	order_date DATE NOT NULL,
	delivery_date DATE
);


INSERT INTO order_details (order_id, customer_name, product_name, ordered_from, ordered_amount, order_date, delivery_date) VALUES
(1, 'John Doe', 'Product 1', 'store', 100.00, '2024-01-29', '2024-02-05'),
(2, 'Jane Doe', 'Product 2', 'app', 200.00, '2024-01-29', '2024-02-06'),
(3, 'John Smith', 'Product 3', 'website', 300.00, '2024-01-30', '2024-02-07'),
(4, 'Jane Smith', 'Product 4', 'store', 400.00, '2024-01-30', '2024-02-08'),
(5, 'John Doe', 'Product 5', 'app', 500.00, '2024-01-31', '2024-02-09'),
(6, 'Jane Doe', 'Product 6', 'website', 600.00, '2024-01-31', '2024-02-10'),
(7, 'John Smith', 'Product 7', 'store', 700.00, '2024-02-01', '2024-02-11'),
(8, 'Jane Smith', 'Product 8', 'app', 800.00, '2024-02-01', '2024-02-12'),
(9, 'John Doe', 'Product 9', 'website', 900.00, '2024-02-02', '2024-02-13'),
(10, 'Jane Doe', 'Product 10', 'store', 1000.00, '2024-02-02', '2024-02-14'),
(11, 'John Smith', 'Product 11', 'app', 1100.00, '2024-02-03', '2024-02-15'),
(12, 'Jane Smith', 'Product 12', 'website', 1200.00, '2024-02-03', '2024-02-16'),
(13, 'John Doe', 'Product 13', 'store', 1300.00, '2024-02-04', '2024-02-17'),
(14, 'Jane Doe', 'Product 14', 'app', 1400.00, '2024-02-04', '2024-02-18'),
(15, 'John Smith', 'Product 15', 'website', 1500.00, '2024-02-05', '2024-02-19'),
(16, 'Jane Smith', 'Product 16', 'store', 1600.00, '2024-02-05', '2024-02-20'),
(17, 'John Doe', 'Product 17', 'app', 1700.00, '2024-02-06', '2024-02-21'),
(18, 'Jane Doe', 'Product 18', 'website', 1800.00, '2024-02-06', '2024-02-22'),
(19, 'John Smith', 'Product 19', 'store', 1900.00, '2024-02-07', '2024-02-23'),
(20, 'Jane Smith', 'Product 20', 'app', 2000.00, '2024-02-07', '2024-02-24'),
(21, 'John Doe', 'Product 21', 'website', 2100.00, '2024-02-08', '2024-02-25'),
(22, 'Jane Doe', 'Product 22', 'store', 2200.00, '2024-02-08', '2024-02-26'),
(23, 'John Smith', 'Product 23', 'app', 2300.00, '2024-02-09', '2024-02-27'),
(24, 'Jane Smith', 'Product 24', 'website', 2400.00, '2024-02-09', '2024-02-28'),
(25, 'John Doe', 'Product 25', 'store', 2500.00, '2024-02-10', '2024-02-29'),
(26, 'Jane Doe', 'Product 26', 'app', 2600.00, '2024-02-10', '2024-03-01');


INSERT INTO order_details (order_id, customer_name, product_name, ordered_from, ordered_amount, order_date, delivery_date) VALUES
(27, 'Jane Smith', 'Product 27', 'app', 2700.00, '2024-02-11', '2024-03-03'),
(28, 'John Doe', 'Product 28', 'website', 2800.00, '2024-02-12', '2024-03-04'),
(29, 'Jane Doe', 'Product 29', 'store', 2900.00, '2024-02-12', '2024-03-05'),
(30, 'John Smith', 'Product 30', 'app', 3000.00, '2024-02-13', '2024-03-06'),
(31, 'Jane Smith', 'Product 31', 'website', 3100.00, '2024-02-13', '2024-03-07'),
(32, 'John Doe', 'Product 32', 'store', 3200.00, '2024-02-14', '2024-03-08'),
(33, 'Jane Doe', 'Product 33', 'app', 3300.00, '2024-02-14', '2024-03-09'),
(34, 'John Smith', 'Product 34', 'website', 3400.00, '2024-02-15', '2024-03-10'),
(35, 'Jane Smith', 'Product 35', 'store', 3500.00, '2024-02-15', '2024-03-11'),
(36, 'John Doe', 'Product 36', 'app', 3600.00, '2024-02-16', '2024-03-12');


INSERT INTO order_details (order_id, customer_name, product_name, ordered_from, ordered_amount, order_date, delivery_date) VALUES
(37, 'Jane Doe', 'Product 37', 'website', 3700.00, '2024-02-16', '2024-03-13'),
(38, 'John Smith', 'Product 38', 'store', 3800.00, '2024-02-17', '2024-03-14'),
(39, 'Jane Smith', 'Product 39', 'app', 3900.00, '2024-02-17', '2024-03-15'),
(40, 'John Doe', 'Product 40', 'website', 4000.00, '2024-02-18', '2024-03-16'),
(41, 'Jane Doe', 'Product 41', 'store', 4100.00, '2024-02-18', '2024-03-17'),
(42, 'John Smith', 'Product 42', 'app', 4200.00, '2024-02-19', '2024-03-18'),
(43, 'Jane Smith', 'Product 43', 'website', 4300.00, '2024-02-19', '2024-03-19'),
(44, 'John Doe', 'Product 44', 'store', 4400.00, '2024-02-20', '2024-03-20'),
(45, 'Jane Doe', 'Product 45', 'app', 4500.00, '2024-02-20', '2024-03-21'),
(46, 'John Smith', 'Product 46', 'website', 4600.00, '2024-02-21', '2024-03-22'),
(47, 'Jane Smith', 'Product 47', 'store', 4700.00, '2024-02-21', '2024-03-23'),
(48, 'John Doe', 'Product 48', 'app', 4800.00, '2024-02-22', '2024-03-24'),
(49, 'Jane Doe', 'Product 49', 'website', 4900.00, '2024-02-22', '2024-03-25'),
(50, 'John Smith', 'Product 50', 'store', 5000.00, '2024-02-23', '2024-03-26');

SELECT * FROM order_details;

SELECT product_name, COUNT(order_id), SUM(ordered_amount) FROM order_details GROUP BY product_name;

ALTER TABLE order_details RENAME COLUMN customer_name TO customer_first_name;

ALTER TABLE order_details ADD COLUMN cancel_date DATE;
