// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.querySelectorAll('.dropdown-submenu .dropdown-toggle').forEach(function (el) {
  el.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var menu = this.parentElement.querySelector('.dropdown-menu');
    if (menu.classList.contains('show')) {
      menu.classList.remove('show');
    } else {
      this.closest('.dropdown-menu').querySelectorAll('.dropdown-menu.show').forEach(function (m) { m.classList.remove('show'); });
      menu.classList.add('show');
    }
  });
});
document.querySelectorAll('.dropdown').forEach(function (drop) {
  drop.addEventListener('hidden.bs.dropdown', function () {
    this.querySelectorAll('.dropdown-menu.show').forEach(function (m) { m.classList.remove('show'); });
  });
});
