import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() userFromHome: any = {};
  @Output() cancelRegistration = new EventEmitter();
  registerForm: FormGroup;
  registerFormFromBuilder: FormGroup;
  maxdate: Date;
  validationerror : string []  = [];

  constructor(private accountservice: AccountService, private toaster: ToastrService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxdate = new Date()
    this.maxdate.setFullYear(this.maxdate.getFullYear() - 18);
  }
  initializeForm(){
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('rpassword')])
    });

    this.registerForm.controls.password.valueChanges.subscribe( () =>{
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });

    this.registerFormFromBuilder = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.maxLength(8), Validators.minLength(4)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    this.registerFormFromBuilder.controls.password.valueChanges.subscribe( () =>{
      this.registerFormFromBuilder.controls.confirmPassword.updateValueAndValidity();
    });

  }


  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      //@ts-ignore
      return control?.value === control?.parent?.controls[matchTo]?.value 
        ? null : {isMatching: true}
    }
  }

  matchValuess(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
        return control.value === (control?.parent?.controls as { [key: string]: AbstractControl })[matchTo].value ? null : { isMatching: true };
    }
}

  register(){
    this.accountservice.register(this.registerFormFromBuilder.value).subscribe( response => {
      this.router.navigateByUrl('/members');
      console.log(response);
    },
    error => {
      this.validationerror = error;
      console.log(error);

    });
    
  }

  cancel(){
    console.log("Cancel from child");
    this.cancelRegistration.emit(false);
  }

  toControl(absCtrl: AbstractControl): FormControl {
    return absCtrl as FormControl;
  }

}
