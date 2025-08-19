import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Inject,
  Output,
} from "@angular/core";
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogModule,
} from "@angular/material/dialog";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { SupplierModel } from "../supplier.model";

@Component({
  selector: "app-supplier-dialog",
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDialogModule,
    MatCheckboxModule,
  ],
  template: `
    <h1 mat-dialog-title>
      {{ data.title }}
    </h1>
    <div mat-dialog-content>
      <form class="supplier-form" [formGroup]="supplierForm">
        <input type="hidden" formControlName="id" />
        
        <mat-form-field [appearance]="'outline'">
          <mat-label>Supplier Name</mat-label>
          <input matInput formControlName="supplierName" maxlength="100" />
          @if (supplierForm.get('supplierName')?.hasError('required')) {
            <mat-error>Supplier name is required</mat-error>
          }
          @if (supplierForm.get('supplierName')?.hasError('maxlength')) {
            <mat-error>Supplier name cannot exceed 100 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Contact Person</mat-label>
          <input matInput formControlName="contactPerson" maxlength="100" />
          @if (supplierForm.get('contactPerson')?.hasError('maxlength')) {
            <mat-error>Contact person cannot exceed 100 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" maxlength="100" />
          @if (supplierForm.get('email')?.hasError('email')) {
            <mat-error>Please enter a valid email address</mat-error>
          }
          @if (supplierForm.get('email')?.hasError('maxlength')) {
            <mat-error>Email cannot exceed 100 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Phone</mat-label>
          <input matInput formControlName="phone" maxlength="20" />
          @if (supplierForm.get('phone')?.hasError('pattern')) {
            <mat-error>Please enter a valid phone number</mat-error>
          }
          @if (supplierForm.get('phone')?.hasError('maxlength')) {
            <mat-error>Phone cannot exceed 20 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Address</mat-label>
          <textarea matInput formControlName="address" maxlength="300" rows="3"></textarea>
          @if (supplierForm.get('address')?.hasError('maxlength')) {
            <mat-error>Address cannot exceed 300 characters</mat-error>
          }
        </mat-form-field>

        <div class="form-row">
          <mat-form-field [appearance]="'outline'">
            <mat-label>City</mat-label>
            <input matInput formControlName="city" maxlength="50" />
            @if (supplierForm.get('city')?.hasError('maxlength')) {
              <mat-error>City cannot exceed 50 characters</mat-error>
            }
          </mat-form-field>

          <mat-form-field [appearance]="'outline'">
            <mat-label>State</mat-label>
            <input matInput formControlName="state" maxlength="50" />
            @if (supplierForm.get('state')?.hasError('maxlength')) {
              <mat-error>State cannot exceed 50 characters</mat-error>
            }
          </mat-form-field>
        </div>

        <div class="form-row">
          <mat-form-field [appearance]="'outline'">
            <mat-label>Country</mat-label>
            <input matInput formControlName="country" maxlength="50" />
            @if (supplierForm.get('country')?.hasError('maxlength')) {
              <mat-error>Country cannot exceed 50 characters</mat-error>
            }
          </mat-form-field>

          <mat-form-field [appearance]="'outline'">
            <mat-label>Postal Code</mat-label>
            <input matInput formControlName="postalCode" maxlength="20" />
            @if (supplierForm.get('postalCode')?.hasError('maxlength')) {
              <mat-error>Postal code cannot exceed 20 characters</mat-error>
            }
          </mat-form-field>
        </div>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Tax Number</mat-label>
          <input matInput formControlName="taxNumber" maxlength="50" />
          @if (supplierForm.get('taxNumber')?.hasError('maxlength')) {
            <mat-error>Tax number cannot exceed 50 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field [appearance]="'outline'">
          <mat-label>Payment Terms (Days)</mat-label>
          <input matInput type="number" formControlName="paymentTerms" min="0" />
          @if (supplierForm.get('paymentTerms')?.hasError('min')) {
            <mat-error>Payment terms must be 0 or greater</mat-error>
          }
        </mat-form-field>

        
      </form>
    </div>
    <div mat-dialog-actions>
      <button
        mat-raised-button
        color="primary"
        (click)="onSubmit()"
        [disabled]="supplierForm.invalid"
      >
        Save
      </button>
      <button mat-raised-button color="warn" (click)="onCanceled()">
        Close
      </button>
    </div>
  `,
  styles: [
    `
      .supplier-form {
        padding: 10px;
        display: flex;
        flex-direction: column;
        gap: 20px;
        min-width: 500px;
      }
      
      mat-form-field {
        width: 100%;
      }
      
      .form-row {
        display: flex;
        gap: 20px;
      }
      
      .form-row mat-form-field {
        flex: 1;
      }
      
      mat-checkbox {
        margin-top: 10px;
      }
      
      mat-dialog-actions {
        justify-content: flex-end;
        gap: 10px;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierDialogComponent {
  @Output() submit = new EventEmitter<SupplierModel>();

  supplierForm: FormGroup = new FormGroup({
    id: new FormControl<number>(0),
    supplierName: new FormControl<string>("", [
      Validators.required,
      Validators.maxLength(100)
    ]),
    contactPerson: new FormControl<string | null>(null, [
      Validators.maxLength(100)
    ]),
    email: new FormControl<string | null>(null, [
      Validators.email,
      Validators.maxLength(100)
    ]),
    phone: new FormControl<string | null>(null, [
      Validators.maxLength(20)
    ]),
    address: new FormControl<string | null>(null, [
      Validators.maxLength(300)
    ]),
    city: new FormControl<string | null>(null, [
      Validators.maxLength(50)
    ]),
    state: new FormControl<string | null>(null, [
      Validators.maxLength(50)
    ]),
    country: new FormControl<string | null>(null, [
      Validators.maxLength(50)
    ]),
    postalCode: new FormControl<string | null>(null, [
      Validators.maxLength(20)
    ]),
    taxNumber: new FormControl<string | null>(null, [
      Validators.maxLength(50)
    ]),
    paymentTerms: new FormControl<number>(30, [
      Validators.min(0)
    ])
  });

  constructor(
    public dialogRef: MatDialogRef<SupplierDialogComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      supplier: SupplierModel | null;
      title: string;
    }
  ) {
    if (data.supplier != null) {
      this.supplierForm.patchValue(data.supplier);
    }
  }

  onCanceled() {
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.supplierForm.valid) {
      const supplier: SupplierModel = this.supplierForm.value as SupplierModel;
      this.submit.emit(supplier);
    }
  }
}