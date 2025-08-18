import { DatePipe, DecimalPipe } from "@angular/common";
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { PurchaseModel } from "../purchase.model";
import { MatSortModule, Sort } from "@angular/material/sort";

@Component({
  selector: "app-purchase-list",
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    DatePipe,
    MatSortModule,
    DecimalPipe
  ],
  styles: [`
    .table-container {
      overflow-x: auto;
      max-width: 100%;
    }
    
    .purchase-table {
      margin-top: 1.5rem;
      min-width: 100%;
      width: max-content; 
    }
    
    /* Make action column sticky to the right */
    .mat-column-action {
      position: sticky;
      right: 0;
      background: white;
      z-index: 1;
      border-left: 1px solid #e0e0e0;
      min-width: 120px; /* Ensure enough space for both buttons */
      width: 120px;
    }
    
    /* Optional: Make first column sticky too for better UX */
    .mat-column-purchaseDate {
      position: sticky;
      left: 0;
      background: white;
      z-index: 1;
      border-right: 1px solid #e0e0e0;
    }
    
    /* Ensure proper spacing for action buttons */
    .action-buttons {
      display: flex;
      gap: 8px;
      justify-content: center;
    }
    
    /* Set minimum widths for columns to prevent squishing */
    .mat-column-productName { min-width: 150px; }
    .mat-column-description { min-width: 200px; }
    .mat-column-purchaseOrderNumber { min-width: 120px; }
    .mat-column-invoiceNumber { min-width: 100px; }
    .mat-column-unitPrice { min-width: 100px; }
    .mat-column-quantity { min-width: 80px; }
    .mat-column-totalPrice { min-width: 120px; }
    .mat-column-receivedDate { min-width: 150px; }
    .mat-column-purchaseDate { min-width: 150px; }
  `],
  template: `
    <div class="table-container">
      <table
        class="mat-elevation-z8 purchase-table"
        mat-table
        [dataSource]="purchases"
        matSort
        (matSortChange)="onSortData($event)"
      >
        <ng-container matColumnDef="purchaseDate">
          <th
            mat-header-cell
            *matHeaderCellDef
            mat-sort-header
            sortActionDescription="sort by purchase date"
          >
            Purchase Date(dd-MM-yyyy)
          </th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.purchaseDate | date : "dd-MM-yyyy HH:mm" }}
          </td>
        </ng-container>

        <ng-container matColumnDef="productName">
          <th
            mat-header-cell
            *matHeaderCellDef
            mat-sort-header
            sortActionDescription="sort by product"
          >
            ProductName
          </th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.productName }}</td>
        </ng-container>

        <ng-container matColumnDef="unitPrice">
          <th mat-header-cell *matHeaderCellDef>Unit Price</th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.unitPrice | number:'1.2-2' }}</td>
        </ng-container>

        <ng-container matColumnDef="quantity">
          <th mat-header-cell *matHeaderCellDef>Quantity</th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.quantity }}</td>
        </ng-container>

        <ng-container matColumnDef="totalPrice">
          <th mat-header-cell *matHeaderCellDef>Total Price</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.unitPrice * purchase.quantity | number:'1.2-2' }}
          </td>
        </ng-container>

        <ng-container matColumnDef="purchaseOrderNumber">
          <th mat-header-cell *matHeaderCellDef>Purchase Order No.</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.purchaseOrderNumber }}
          </td>
        </ng-container>

        <ng-container matColumnDef="invoiceNumber">
          <th mat-header-cell *matHeaderCellDef>Invoice#</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.invoiceNumber }}
          </td>
        </ng-container>

        <ng-container matColumnDef="receivedDate">
          <th mat-header-cell *matHeaderCellDef>Received Date</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.receivedDate | date: "dd-MM-yyyy HH:mm" }}
          </td>
        </ng-container>

        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.description }}
          </td>
        </ng-container>

        <ng-container matColumnDef="action">
          <th mat-header-cell *matHeaderCellDef>Action</th>
          <td mat-cell *matCellDef="let purchase">
            <div class="action-buttons">
              <button
                mat-mini-fab
                color="primary"
                aria-label="Edit"
                (click)="edit.emit(purchase)"
              >
                <mat-icon>edit</mat-icon>
              </button>
              <button
                mat-mini-fab
                color="warn"
                aria-label="Delete"
                (click)="delete.emit(purchase)"
              >
                <mat-icon>delete</mat-icon>
              </button>
            </div>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
    </div>
  `
})
export class PurchaseListComponent {
  @Input({ required: true }) purchases!: PurchaseModel[];
  @Output() edit = new EventEmitter<PurchaseModel>();
  @Output() delete = new EventEmitter<PurchaseModel>();
  @Output() sort = new EventEmitter<{
    sortColumn: string;
    sortDirection: "asc" | "desc";
  }>();

  onSortData(sortData: Sort) {
    const sortColumn = sortData.active;
    const sortDirection = sortData.direction as "asc" | "desc";
    this.sort.emit({ sortColumn, sortDirection });
  }

  displayedColumns = [
    "purchaseDate",
    "productName",
    "unitPrice",
    "quantity",
    "description",
    "purchaseOrderNumber",
    "invoiceNumber",
    "receivedDate",
    "totalPrice",
    "action",
  ];
}