document.addEventListener('DOMContentLoaded', () => {
  "use strict";

  const selectHeader = document.querySelector('#header');
  if (selectHeader) {
    document.addEventListener('scroll', () => {
      window.scrollY > 100 ? selectHeader.classList.add('sticked') : selectHeader.classList.remove('sticked');
    });
  }

  const nav_items = document.querySelector('.container');
    if (nav_items.id != "") {
        var nav_items_active = "#nav_items_" + nav_items.id;
        //console.log(nav_items_active);
        var target = document.querySelector(nav_items_active);
        target.classList.add("nav_items_active");
        //console.log(target);
    }

  const mobileNavToogleButton = document.querySelector('.mobile-nav-toggle');

  if (mobileNavToogleButton) {
    mobileNavToogleButton.addEventListener('click', function(event) {
      event.preventDefault();
      mobileNavToogle();
    });
  }

  function mobileNavToogle() {
    document.querySelector('body').classList.toggle('mobile-nav-active');
    mobileNavToogleButton.classList.toggle('bi-list');
    mobileNavToogleButton.classList.toggle('bi-x');
  }

  document.querySelectorAll('#navbar a').forEach(navbarlink => {

    if (!navbarlink.hash) return;

    let section = document.querySelector(navbarlink.hash);
    if (!section) return;

    navbarlink.addEventListener('click', () => {
      if (document.querySelector('.mobile-nav-active')) {
        mobileNavToogle();
      }
    });
  });

  const navDropdowns = document.querySelectorAll('.navbar .dropdown > a');

  navDropdowns.forEach(el => {
    el.addEventListener('click', function(event) {
      if (document.querySelector('.mobile-nav-active')) {
        event.preventDefault();
        this.classList.toggle('active');
        this.nextElementSibling.classList.toggle('dropdown-active');

        let dropDownIndicator = this.querySelector('.dropdown-indicator');
        dropDownIndicator.classList.toggle('bi-chevron-up');
        dropDownIndicator.classList.toggle('bi-chevron-down');
      }
    })
  });

  const scrollTop = document.querySelector('.scroll-top');
  if (scrollTop) {
    const togglescrollTop = function() {
      window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
    }
    window.addEventListener('load', togglescrollTop);
    document.addEventListener('scroll', togglescrollTop);
    scrollTop.addEventListener('click', window.scrollTo({
      top: 0,
      behavior: 'smooth'
    }));
  }

  var swiper = new Swiper(".sliderFeaturedPosts", {
    spaceBetween: 0,
    speed: 500,
    centeredSlides: true,
    loop: true,
    slideToClickedSlide: true,
    autoplay: {
      delay: 3000,
      disableOnInteraction: false,
    },
    pagination: {
      el: ".swiper-pagination",
      clickable: true,
    },
    navigation: {
      nextEl: ".custom-swiper-button-next",
      prevEl: ".custom-swiper-button-prev",
    },
  });

  const searchOpen = document.querySelector('.js-search-open');
  const searchWrap = document.querySelector(".js-search-form-wrap");

  searchOpen.addEventListener("click", (e) => {
    e.preventDefault();
    searchWrap.classList.toggle("active");
  });

  const glightbox = GLightbox({
    selector: '.glightbox'
  });

  function aos_init() {
    AOS.init({
      duration: 1000,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });
  }
  window.addEventListener('load', () => {
    aos_init();
  });


  /*const tologin = document.querySelector('#tologin');
  const backtosing = document.querySelector('#backtosing');
  const form_sign = document.querySelector('#form_sign');
  const form_login = document.querySelector('#form_login');

 if (tologin) {
   tologin.addEventListener("click", (e) => {
   e.preventDefault();
   form_sign.classList.add("login-collapse");
   form_login.classList.remove("login-collapse");
   });
 }

 if (backtosing) {
   backtosing.addEventListener("click", (e) => {
   e.preventDefault();
   form_sign.classList.remove("login-collapse");
   form_login.classList.add("login-collapse");
   });
 }*/

});