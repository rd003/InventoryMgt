create database inventory_mgt;

-- category table
create table category
(
    id serial,
    create_date timestamptz not null default current_timestamp,
    update_date timestamptz not null default current_timestamp,
    is_deleted boolean default false,
    category_name varchar(50) not null,
    category_id int,

    constraint pk_category_id primary key (id),
    constraint fk_category_parent foreign key (category_id) references category(id)
)

-- supplier
create table supplier
(
    id serial primary key,
    create_date timestamptz default current_timestamp,
    update_date timestamptz default current_timestamp,
    is_deleted bool default false,
    supplier_name varchar(100) not null,
    contact_person varchar(100),
    email varchar(100),
    phone varchar(20),
    address varchar(300),
    city varchar(50),
    "state" varchar(50),
    country varchar(50),
    postal_code varchar(20),
    tax_number varchar(50),
    payment_terms int DEFAULT 30,
    is_active bool DEFAULT TRUE
)

-- product
CREATE TABLE product
(
    id SERIAL PRIMARY KEY,
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOL DEFAULT FALSE,
    product_name VARCHAR(50) NOT NULL,
    category_id INT NOT NULL REFERENCES category(id),
    price DECIMAL(18, 2) NOT NULL,
    supplier_id INT REFERENCES supplier(id)
);

-- purcase
CREATE TABLE purchase
(
    id SERIAL PRIMARY KEY,
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOL DEFAULT FALSE,
    product_id INT REFERENCES product(id),
    supplier_id INT REFERENCES supplier(id),
    purchase_date TIMESTAMPTZ NOT NULL,
    quantity decimal(10,3) NOT NULL,
    description VARCHAR(100),
    unit_price DECIMAL(18, 2) NOT NULL,
    purchase_order_number VARCHAR(50),
    invoice_number VARCHAR(50),
    received_date TIMESTAMPTZ
);

-- sale
CREATE TABLE sale
(
    id SERIAL PRIMARY KEY,
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOL DEFAULT FALSE,
    product_id INT NOT NULL REFERENCES product(id),
    selling_date TIMESTAMPTZ NOT NULL,
    quantity DECIMAL(10, 3),
    description VARCHAR(100) NOT NULL,
    price DECIMAL(18, 2) NOT NULL
);

-- stock
-- note: boolen and bool , integer and int are same
CREATE TABLE stock
(
    id SERIAL PRIMARY KEY,
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    product_id INTEGER UNIQUE REFERENCES product(id),
    quantity decimal(10,3) NOT NULL
);


-- indexes

-- Supplier indexes
CREATE INDEX idx_supplier_name ON supplier(supplier_name);
CREATE INDEX idx_supplier_email ON supplier(email);
CREATE INDEX idx_supplier_active ON supplier(is_active) WHERE is_active = TRUE;
CREATE INDEX idx_supplier_not_deleted ON supplier(is_deleted) WHERE is_deleted = FALSE;

-- Category indexes
CREATE INDEX idx_category_name ON category(category_name);
CREATE INDEX idx_category_parent ON category(category_id) WHERE category_id IS NOT NULL;
CREATE INDEX idx_category_active ON category(is_deleted) WHERE is_deleted = FALSE;

-- Product indexes
CREATE INDEX idx_product_category ON product(category_id);
CREATE INDEX idx_product_supplier ON product(supplier_id);
CREATE INDEX idx_product_name ON product(product_name);
CREATE INDEX idx_product_price ON product(price);
CREATE INDEX idx_product_active ON product(is_deleted) WHERE is_deleted = FALSE;

-- Purchase indexes
CREATE INDEX idx_purchase_product ON purchase(product_id);
CREATE INDEX idx_purchase_supplier ON purchase(supplier_id);
CREATE INDEX idx_purchase_date ON purchase(purchase_date);
CREATE INDEX idx_purchase_product_date ON purchase(product_id, purchase_date);
CREATE INDEX idx_purchase_supplier_date ON purchase(supplier_id, purchase_date);
CREATE INDEX idx_purchase_order ON purchase(purchase_order_number);
CREATE INDEX idx_purchase_invoice ON purchase(invoice_number);
CREATE INDEX idx_purchase_active ON purchase(is_deleted) WHERE is_deleted = FALSE;

-- Sale indexes
CREATE INDEX idx_sale_product ON sale(product_id);
CREATE INDEX idx_sale_date ON sale(selling_date);
CREATE INDEX idx_sale_product_date ON sale(product_id, selling_date);
CREATE INDEX idx_sale_active ON sale(is_deleted) WHERE is_deleted = FALSE;

-- Stock indexes (product_id already has unique constraint)
CREATE INDEX idx_stock_quantity ON stock(quantity);
CREATE INDEX idx_stock_active ON stock(is_deleted) WHERE is_deleted = FALSE;

-- Composite indexes for common queries
CREATE INDEX idx_product_category_active ON product(category_id, is_deleted);
CREATE INDEX idx_product_supplier_active ON product(supplier_id, is_deleted);
CREATE INDEX idx_purchase_date_product ON purchase(purchase_date, product_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_purchase_supplier_product ON purchase(supplier_id, product_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_sale_date_product ON sale(selling_date, product_id) WHERE is_deleted = FALSE;

