(() => {
  const lines = document.querySelector('#sale-lines');
  const template = document.querySelector('#line-template');
  const add = document.querySelector('#add-line');
  function rename() {
    lines.querySelectorAll('.sale-line').forEach((line, index) => {
      line.querySelector('select').name = `Items[${index}].BicycleId`;
      line.querySelector('input').name = `Items[${index}].Quantity`;
    });
  }
  add.addEventListener('click', () => { lines.append(template.content.cloneNode(true)); rename(); });
  lines.addEventListener('click', event => {
    if (!event.target.classList.contains('remove-line')) return;
    if (lines.children.length > 1) event.target.closest('.sale-line').remove();
    else { event.target.closest('.sale-line').querySelector('select').value = ''; event.target.closest('.sale-line').querySelector('input').value = 1; }
    rename();
  });
  rename();
})();
