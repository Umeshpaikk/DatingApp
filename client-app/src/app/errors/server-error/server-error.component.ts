import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any = null;
  constructor(private route: Router) { 
    const navigations = route.getCurrentNavigation();
    this.error = navigations?.extras?.state?.error;

  }

  ngOnInit(): void {
  }

}
