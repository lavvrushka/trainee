
import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Staff {
  photo: string;
  name: string;
  position: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
   imports: [
    CommonModule           
   
  ]
})
export class HomeComponent {
  @ViewChild('carousel', { static: true }) carousel!: ElementRef<HTMLDivElement>;

  staffList: Staff[] = [
    { photo: 'assets/staff/staff1.jpg', name: 'Др. Лобанов', position: 'Терапевт' },
    { photo: 'assets/staff/staff2.jpg', name: 'Др. Романенко', position: 'Кардиолог' },
    { photo: 'assets/staff/staff3.jpg', name: 'Др. Черноус', position: 'Педиатр' },
    { photo: 'assets/staff/staff5.jpg', name: 'Др. Левин', position: 'Терапевт' },
    { photo: 'assets/staff/staff4.jpg', name: 'Зав. Быков', position: 'Зав. отделением' },
  ];

  scrollPrev() {
    this.carousel.nativeElement.scrollBy({ left: -300, behavior: 'smooth' });
  }
  scrollNext() {
    this.carousel.nativeElement.scrollBy({ left: 300, behavior: 'smooth' });
  }
}
