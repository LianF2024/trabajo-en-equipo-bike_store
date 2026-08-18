document.querySelectorAll('form[data-confirm]').forEach(form => {
  form.addEventListener('submit', event => {
    if (!window.confirm(form.dataset.confirm)) event.preventDefault();
  });
});

const menuToggle = document.getElementById('menu-toggle');
const collapseMenu = document.getElementById('collapse-menu');
const sidebarOverlay = document.getElementById('sidebar-overlay');

menuToggle?.addEventListener('click', () => {
  if (window.innerWidth <= 768) document.body.classList.toggle('sidebar-open');
  else document.body.classList.toggle('sidebar-collapsed');
});

collapseMenu?.addEventListener('click', () => {
  if (window.innerWidth <= 768) document.body.classList.remove('sidebar-open');
  else document.body.classList.toggle('sidebar-collapsed');
});

sidebarOverlay?.addEventListener('click', () => document.body.classList.remove('sidebar-open'));

document.querySelectorAll('.sidebar-menu a').forEach(link => {
  link.addEventListener('click', () => document.body.classList.remove('sidebar-open'));
});
