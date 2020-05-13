import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-Weather',
  templateUrl: './Weather.component.html',
  styleUrls: ['./Weather.component.css']
})
export class WeatherComponent implements OnInit {
weather: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getWeather();
    console.log(this.weather);
  }
  getWeather() {
    this.http.get('http://localhost:5000/weatherforecast').subscribe(res => {
      this.weather = JSON.stringify(res);
      //console.log(res);
    }, error => {
      console.log(error);
    }
    );
  }
}

