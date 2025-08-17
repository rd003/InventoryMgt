import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from "@angular/core";
import { Product } from "../product.model";
import { MatTableModule } from "@angular/material/table";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatSortModule, Sort } from "@angular/material/sort";

@Component({
  selector: "app-product-list",
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSortModule,
  ],
  styles: [``],
  template: `
    <table
      style=" margin-top: 1.5rem;"
      mat-table
      [dataSource]="products"
      class="mat-elevation-z8"
      matSort
      (matSortChange)="onSortData($event)"
    >
      <ng-container matColumnDef="productName">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="sort by product"
        >
          Product
        </th>
        <td mat-cell *matCellDef="let element">
          {{ element.productName }}
        </td>
      </ng-container>

      <ng-container matColumnDef="sku">
        <th
          mat-header-cell
          *matHeaderCellDef
        >
          SKU
        </th>
        <td mat-cell *matCellDef="let element">
          {{ element.sku }}
        </td>
      </ng-container>

      <ng-container matColumnDef="categoryName">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="sort by category"
        >
          Category
        </th>
        <td mat-cell *matCellDef="let element">{{ element.categoryName }}</td>
      </ng-container>

      <ng-container matColumnDef="supplierName">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="sort by supplier"
        >
          Supplier
        </th>
        <td mat-cell *matCellDef="let element">{{ element.supplierName }}</td>
      </ng-container>

      <ng-container matColumnDef="price">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="sort by price"
        >
          Price
        </th>
        <td mat-cell *matCellDef="let element">
          {{ element.price }}
        </td>
      </ng-container>


      <ng-container matColumnDef="action">
        <th mat-header-cell *matHeaderCellDef>Action</th>
        <td mat-cell *matCellDef="let element">
          <div style="display: flex;gap:5px;align-items:center">
            <button
              mat-mini-fab
              color="primary"
              aria-label="Edit"
              (click)="edit.emit(element)"
            >
              <mat-icon>edit</mat-icon>
            </button>

            <button
              mat-mini-fab
              color="warn"
              aria-label="Delete"
              (click)="delete.emit(element)"
            >
              <mat-icon>delete</mat-icon>
            </button>
          </div>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
    </table>
  `
})
export class ProductListComponent {
  @Input({ required: true }) products!: Product[];
  @Output() delete = new EventEmitter<Product>();
  @Output() edit = new EventEmitter<Product>();
  @Output() sort = new EventEmitter<{
    sortColumn: string;
    sortDirection: "asc" | "desc";
  }>();

  displayedColumns = [
    "productName",
    "sku",
    "price",
    "categoryName",
    "supplierName",
    "action",
  ];

  onSortData(sortData: Sort) {
    const sortColumn = sortData.active;
    const sortDirection = sortData.direction as "asc" | "desc";
    this.sort.emit({ sortColumn, sortDirection });
  }
}
