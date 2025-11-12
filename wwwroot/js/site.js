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
      menu.classList.add('show');
    }
  });
});
document.querySelectorAll('.dropdown').forEach(function (drop) {
  drop.addEventListener('hidden.bs.dropdown', function () {
    this.querySelectorAll('.dropdown-menu.show').forEach(function (m) { m.classList.remove('show'); });
  });
});
document.querySelectorAll('.tree-toggle').forEach(function (btn) {
  btn.addEventListener('click', function (e) {
    e.preventDefault();
    var li = this.closest('li');
    if (!li) return;
    var children = li.querySelector('.tree-children');
    if (!children) return;
    var expanded = this.getAttribute('aria-expanded') === 'true';
    this.setAttribute('aria-expanded', expanded ? 'false' : 'true');
    if (expanded) {
      children.classList.remove('show');
    } else {
      children.classList.add('show');
    }
    var icon = this.querySelector('i');
    if (icon) {
      if (expanded) {
        icon.classList.remove('bi-chevron-down');
        icon.classList.add('bi-chevron-right');
      } else {
        icon.classList.remove('bi-chevron-right');
        icon.classList.add('bi-chevron-down');
      }
    }
  });
});
var expandAll = document.getElementById('expandAllGroups');
if (expandAll) {
  expandAll.addEventListener('click', function () {
    document.querySelectorAll('.tree-children').forEach(function (c) { c.classList.add('show'); });
    document.querySelectorAll('.tree-toggle').forEach(function (btn) {
      btn.setAttribute('aria-expanded', 'true');
      var icon = btn.querySelector('i');
      if (icon) {
        icon.classList.remove('bi-chevron-right');
        icon.classList.add('bi-chevron-down');
      }
    });
  });
}
var collapseAll = document.getElementById('collapseAllGroups');
if (collapseAll) {
  collapseAll.addEventListener('click', function () {
    document.querySelectorAll('.tree-children').forEach(function (c) { c.classList.remove('show'); });
    document.querySelectorAll('.tree-toggle').forEach(function (btn) {
      btn.setAttribute('aria-expanded', 'false');
      var icon = btn.querySelector('i');
      if (icon) {
        icon.classList.remove('bi-chevron-down');
        icon.classList.add('bi-chevron-right');
      }
    });
  });
}
document.addEventListener('click', function (e) {
  var t = e.target;
  if (t && t.classList.contains('group-item')) {
    var gid = t.getAttribute('data-group-id');
    if (!gid) return;
    fetch('/Estoque/Grupos/Index?handler=Produtos&id=' + encodeURIComponent(gid))
      .then(function (r) { return r.json(); })
      .then(function (items) {
        var container = document.getElementById('grupoProdutos');
        var actions = document.getElementById('produtosAcoes');
        if (!container) return;
        var html = '';
        html += '<table class="table table-dark table-striped align-middle">';
        html += '<thead><tr><th>Id</th><th>Nome</th></tr></thead><tbody>';
        items.forEach(function (p) {
          html += '<tr><td>' + p.id + '</td><td>' + p.nome + '</td></tr>';
        });
        html += '</tbody></table>';
        container.innerHTML = html;
        if (actions) {
          actions.innerHTML = '';
        }
      });
  }
});
