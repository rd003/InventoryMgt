using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryMgt.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    category_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_parent",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "supplier",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    supplier_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contact_person = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    state = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tax_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_terms = table.Column<int>(type: "integer", nullable: true, defaultValue: 30),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("supplier_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    product_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    supplier_id = table.Column<int>(type: "integer", nullable: true),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pkey", x => x.id);
                    table.ForeignKey(
                        name: "product_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "product_supplier_id_fkey",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purchase",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    supplier_id = table.Column<int>(type: "integer", nullable: true),
                    purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    purchase_order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("purchase_pkey", x => x.id);
                    table.ForeignKey(
                        name: "purchase_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "purchase_supplier_id_fkey",
                        column: x => x.supplier_id,
                        principalTable: "supplier",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    selling_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_pkey", x => x.id);
                    table.ForeignKey(
                        name: "sale_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stock_pkey", x => x.id);
                    table.ForeignKey(
                        name: "stock_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_category_active",
                table: "category",
                column: "is_deleted",
                filter: "(is_deleted = false)");

            migrationBuilder.CreateIndex(
                name: "idx_category_name",
                table: "category",
                column: "category_name");

            migrationBuilder.CreateIndex(
                name: "idx_category_parent",
                table: "category",
                column: "category_id",
                filter: "(category_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "idx_product_active",
                table: "product",
                column: "is_deleted",
                filter: "(is_deleted = false)");

            migrationBuilder.CreateIndex(
                name: "idx_product_category",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "idx_product_category_active",
                table: "product",
                columns: new[] { "category_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "idx_product_name",
                table: "product",
                column: "product_name");

            migrationBuilder.CreateIndex(
                name: "idx_product_price",
                table: "product",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "idx_product_supplier",
                table: "product",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "idx_product_supplier_active",
                table: "product",
                columns: new[] { "supplier_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "product_sku_key",
                table: "product",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_product_id",
                table: "purchase",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_supplier_id",
                table: "purchase",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "idx_sale_active",
                table: "sale",
                column: "is_deleted",
                filter: "(is_deleted = false)");

            migrationBuilder.CreateIndex(
                name: "idx_sale_date",
                table: "sale",
                column: "selling_date");

            migrationBuilder.CreateIndex(
                name: "idx_sale_date_product",
                table: "sale",
                columns: new[] { "selling_date", "product_id" },
                filter: "(is_deleted = false)");

            migrationBuilder.CreateIndex(
                name: "idx_sale_product",
                table: "sale",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "idx_sale_product_date",
                table: "sale",
                columns: new[] { "product_id", "selling_date" });

            migrationBuilder.CreateIndex(
                name: "idx_stock_active",
                table: "stock",
                column: "is_deleted",
                filter: "(is_deleted = false)");

            migrationBuilder.CreateIndex(
                name: "idx_stock_quantity",
                table: "stock",
                column: "quantity");

            migrationBuilder.CreateIndex(
                name: "stock_product_id_key",
                table: "stock",
                column: "product_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_supplier_active",
                table: "supplier",
                column: "is_active",
                filter: "(is_active = true)");

            migrationBuilder.CreateIndex(
                name: "idx_supplier_email",
                table: "supplier",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_supplier_name",
                table: "supplier",
                column: "supplier_name");

            migrationBuilder.CreateIndex(
                name: "idx_supplier_not_deleted",
                table: "supplier",
                column: "is_deleted",
                filter: "(is_deleted = false)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchase");

            migrationBuilder.DropTable(
                name: "sale");

            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "supplier");
        }
    }
}
