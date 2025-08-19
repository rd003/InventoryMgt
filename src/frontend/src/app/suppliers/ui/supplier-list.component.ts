import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from "@angular/core";
import { SupplierModel } from "../supplier.model";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from "@angular/material/table";
import { MatSortModule, Sort } from "@angular/material/sort";

@Component({
    selector: 'app-supplier-list',
    imports: [
        MatTableModule,
        MatButtonModule,
        MatIconModule,
        MatSortModule,
    ],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
    <div class="table-container">
      <table
        class="mat-elevation-z8 purchase-table"
        mat-table
        [dataSource]="suppliers"
        matSort
        (matSortChange)="onSortData($event)"
      >
        <ng-container matColumnDef="supplierName">
          <th
            mat-header-cell
            *matHeaderCellDef
            mat-sort-header
            sortActionDescription="sort by supplier name"
          >
            Supplier
          </th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.supplierName }}
          </td>
        </ng-container>

        <ng-container matColumnDef="contactPerson">
          <th
            mat-header-cell
            *matHeaderCellDef
            mat-sort-header
            sortActionDescription="sort by contact-person"
          >
            Contact Person
          </th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.contactPerson }}</td>
        </ng-container>

        <ng-container matColumnDef="email">
          <th mat-header-cell *matHeaderCellDef>Email</th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.email}}</td>
        </ng-container>

        <ng-container matColumnDef="phone">
          <th mat-header-cell *matHeaderCellDef>Phone</th>
          <td mat-cell *matCellDef="let purchase">{{ purchase.phone }}</td>
        </ng-container>

        <ng-container matColumnDef="address">
          <th mat-header-cell *matHeaderCellDef>Address</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.address }}
          </td>
        </ng-container>

        <ng-container matColumnDef="city">
          <th mat-header-cell *matHeaderCellDef>City</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.city }}
          </td>
        </ng-container>

        <ng-container matColumnDef="state">
          <th mat-header-cell *matHeaderCellDef>State</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.state }}
          </td>
        </ng-container>

        <ng-container matColumnDef="country">
          <th mat-header-cell *matHeaderCellDef>Country</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.country }}
          </td>
        </ng-container>

        <ng-container matColumnDef="postalCode">
          <th mat-header-cell *matHeaderCellDef>PostalCode</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.postalCode }}
          </td>
        </ng-container>

         <ng-container matColumnDef="taxNumber">
          <th mat-header-cell *matHeaderCellDef>Tax No.</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.taxNumber }}
          </td>
        </ng-container>

         <ng-container matColumnDef="paymentTerms">
          <th mat-header-cell *matHeaderCellDef>Payment terms</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.paymentTerms }} days
          </td>
        </ng-container>

         <ng-container matColumnDef="isActive">
          <th mat-header-cell *matHeaderCellDef>Active</th>
          <td mat-cell *matCellDef="let purchase">
            {{ purchase.isActive? "Yes":"No" }} 
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
    `,
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
    .mat-column-supplierName { min-width: 150px; }
    .mat-column-contactPerson { min-width: 200px; }
    .mat-column-email { min-width: 120px; }
    .mat-column-phone { min-width: 100px; }
    .mat-column-address { min-width: 100px; }
    .mat-column-city { min-width: 80px; }
    .mat-column-state { min-width: 120px; }
    .mat-column-country { min-width: 150px; }
    .mat-column-postalCode { min-width: 150px; }
    .mat-column-taxNumber { min-width: 150px; }
    .mat-column-paymentTerms { min-width: 150px; }
    .mat-column-isActive { min-width: 150px; }
        `]
})
export class SupplierListComponent {
    @Input({ required: true }) suppliers: readonly SupplierModel[] = [];
    @Output() edit = new EventEmitter<SupplierModel>();
    @Output() delete = new EventEmitter<SupplierModel>();
    @Output() sort = new EventEmitter<{
        sortColumn: string;
        sortDirection: "asc" | "desc";
    }>();

    onSortData = (sortData: Sort) => {
        const sortColumn = sortData.active;
        const sortDirection = sortData.direction as "asc" | "desc";
        this.sort.emit({ sortColumn, sortDirection });
    }

    displayedColumns = [
        'supplierName',
        'contactPerson',
        'email',
        'phone',
        'address',
        'city',
        'state',
        'country',
        'postalCode',
        'taxNumber',
        'paymentTerms',
        'isActive',
        'action',
    ];
}