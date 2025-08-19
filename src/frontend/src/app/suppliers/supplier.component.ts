import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { SupplierStore } from "./supplier.store";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { SupplierListComponent } from "./ui/supplier-list.component";
import { SupplierModel } from "./supplier.model";
import { SupplierFilterComponent } from "./ui/supplier-filter.comonent";
import { SupplierPaginatorComponent } from "./ui/supplier-paginator.component";

@Component({
    selector: 'app-supplier',
    imports: [
        MatProgressSpinnerModule,
        MatButtonModule,
        SupplierListComponent,
        SupplierFilterComponent,
        SupplierPaginatorComponent
    ],
    providers: [SupplierStore],
    template: `
       <div style="display: flex;align-items:center;gap:5px;margin-bottom:8px">
        <span style="font-size: 26px;font-weight:bold"> Suppliers </span>
        <button style="margin:10px 0px"
        mat-raised-button
        color="primary"
        (click)="onAddUpdate('Add Product')"
      >
        Add More
      </button>
      </div>

      @if(store.loading()){
        <div class="spinner-center">
          <mat-spinner diameter="50"></mat-spinner>
        </div>
      }
      
    <app-supplier-filter (filter)="onFilter($event)"/>

     @if(store.suppliers() && store.suppliers().length > 0){

         <app-supplier-list [suppliers]="store.suppliers()" (sort)="onSort($event)" (edit)="onEdit($event)" (delete)="onDelete($event)"/>

         <app-supplier-paginator [totalRecords]="store.totalRecords()" (pageSelect)="onPageSelect($event)"/>
     }
     @else {
         <p style="margin-top:20px;font-size:21px">
          No records found
        </p>
     }
    `,
    styles: [``],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierComponent {
    store = inject(SupplierStore);

    onPageSelect = (pageData: { page: number; limit: number }) => {
        this.store.setPagination(pageData);
    }

    onFilter = (searchTerm: string) => this.store.setSearch(searchTerm);

    onSort = (sortingData: { sortColumn: string; sortDirection: "asc" | "desc" }) => this.store.setSorting(sortingData);

    onEdit = (supplier: SupplierModel) => {
        console.log(supplier);
    }

    onDelete = (supplier: SupplierModel) => {
        if (!confirm('Are you sure to delete supplier ' + supplier.supplierName))
            return;
        console.log(supplier);
    }

    onAddUpdate = (title: string) => {

    }
}