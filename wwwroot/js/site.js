// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const sidebar = document.getElementById('sidebar');
const overlay = document.getElementById('overlay');
const menuBtn = document.getElementById('menuBtn');
function openSidebar() {
  sidebar.classList.add('open');
  overlay.classList.add('visible');
  document.body.style.overflow = 'hidden';
}
function closeSidebar() {
  sidebar.classList.remove('open');
  overlay.classList.remove('visible');
  document.body.style.overflow = '';
}
menuBtn.addEventListener('click', openSidebar);
overlay.addEventListener('click', closeSidebar);
window.addEventListener('keydown', (e)=>{
  if(e.key === "Escape") closeSidebar();
});

 // Скрой меню при прокрутке вниз, покажи при прокрутке вверх
 let lastScrollY = window.scrollY, ticking = false;
 const nav = document.querySelector('.nav-hero');
 const hero = document.querySelector('.hero');
 function onScroll() {
   const currentY = window.scrollY;
   if(currentY > lastScrollY+8) {
     nav.classList.add('hide-menu'); hero.classList.add('hide-menu');
   } else if(currentY < lastScrollY-8) {
     nav.classList.remove('hide-menu'); hero.classList.remove('hide-menu');
   }
   lastScrollY = currentY; ticking = false;
 }
 window.addEventListener('scroll', () => {
   if (!ticking) { window.requestAnimationFrame(onScroll); ticking = true; }
 });