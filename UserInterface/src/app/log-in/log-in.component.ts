import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.scss'],
  animations: [
    
  ]
})
export class LogInComponent implements OnInit {

  username = '';
  password = '';
  hide = true;
  
  constructor() { }

  ngOnInit(): void {
  }

  logIn(){
  }

}
