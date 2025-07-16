import { Component, HostListener } from '@angular/core';
import { CommonModule }            from '@angular/common';
import { RouterModule }            from '@angular/router';
import { FontAwesomeModule }       from '@fortawesome/angular-fontawesome';
import { faMapMarkerAlt, faEnvelope, faPhoneAlt, faSearch } from '@fortawesome/free-solid-svg-icons';
import { faFacebookF, faTwitter, faInstagram } from '@fortawesome/free-brands-svg-icons';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FontAwesomeModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  faMapMarkerAlt = faMapMarkerAlt;
  faEnvelope     = faEnvelope;
  faPhoneAlt     = faPhoneAlt;
  faSearch       = faSearch;

  socials = [
    { icon: 'facebook-f', url: 'https://facebook.com' },
    { icon: 'twitter',     url: 'https://twitter.com' },
    { icon: 'instagram',   url: 'https://instagram.com' }
  ];

  menu = [
    { label: 'HOME',        link: '/',           children: [{label:'Home1',link:'/'},{label:'Home2',link:'/'}] },
    { label: 'ABOUT',       link: '/about',      children: [{label:'About1',link:'/about'},{label:'Team',link:'/team'}] },
    { label: 'SERVICE',     link: '/service' },
    { label: 'GALLERY',     link: '/gallery' },
    { label: 'BLOG',        link: '/blog',        children: [{label:'Blog1',link:'/blog'},{label:'Post',link:'/blog-post'}] },
    { label: 'CONTACT',     link: '/contact' }
  ];

  mobileMenuOpen = false;
  searchExpanded = false;
  scrolled = false;

  @HostListener('window:scroll', [])
  onWindowScroll() {
    this.scrolled = window.scrollY > 50;
  }

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  expandSearch(open: boolean) {
    this.searchExpanded = open;
  }

  onSearch(query: string) {

    console.log('Search for:', query);
  }
}
