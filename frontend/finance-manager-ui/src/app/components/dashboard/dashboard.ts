import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class DashboardComponent implements OnInit {
  expenses: number = 0;
  income: number = 0;
  savings: number = 0;

  pieChartData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Expenses', 'Income', 'Savings'],
    datasets: [
      {
        data: [0, 0, 0],
        backgroundColor: ['#f87171', '#34d399', '#60a5fa'],
      },
    ],
  };

  pieChartOptions: ChartOptions<'pie'> = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top',
      },
    },
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.http.get<any>('http://localhost:5224/api/Dashboard/summary').subscribe({
      next: (res) => {
        this.expenses = res.expenses;
        this.income = res.income;
        this.savings = res.savings;

        this.pieChartData.datasets[0].data = [
          this.expenses,
          this.income,
          this.savings,
        ];
      },
      error: (err) => {
        console.error('Dashboard API error:', err);
      },
    });
  }
}
