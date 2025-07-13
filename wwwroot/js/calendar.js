document.addEventListener('DOMContentLoaded', () => {
  const centerSel = document.getElementById('centerSelect'),
        typeSel   = document.getElementById('typeSelect'),
        calEl     = document.getElementById('calendar'),
        timesEl   = document.getElementById('timeslots'),
        form      = document.getElementById('bookingForm'),
        submitBtn = document.getElementById('submitBtn');

  let slots = {}, selDate = null, selTime = null, selBtn = null;

  // 1) загрузить центры и типы услуг
  Promise.all([
    fetch('/api/servicecenters').then(r=>r.json()),
    fetch('/api/servicetypes')   .then(r=>r.json())
  ]).then(([centers, types]) => {
    centers.forEach(c => centerSel.append(new Option(c.name, c.serviceCenterId)));
    types.forEach(t => {
      const o = new Option(`${t.name} (${t.durationHours}ч)`, t.serviceTypeId);
      o.dataset.duration = t.durationHours;
      typeSel.append(o);
    });
    loadSlots();
  });

  // 2) инициализация календаря
  const calendar = new FullCalendar.Calendar(calEl, {
    initialView:'dayGridMonth',
    locale:'ru',
    datesSet: loadSlots,
    dateClick: info => onDateClick(info.dateStr, info.dayEl)
  });
  calendar.render();

  centerSel.onchange = () => { resetDate(); resetTime(); loadSlots(); };
  typeSel.onchange = () => { resetDate(); resetTime(); loadSlots(); submitBtn.disabled = !(selDate && selTime);};
  //typeSel.onchange   = () => submitBtn.disabled = !(selDate && selTime);

  function getRange() {
    const v = calendar.view;
    return { start: v.currentStart.toISOString().slice(0,10),
             end:   v.currentEnd  .toISOString().slice(0,10) };
  }

  // 3) загрузка слотов с учётом duration
  async function loadSlots() {
    resetDate(); resetTime();
    const { start, end } = getRange();
    const url = `/api/slots?center=${centerSel.value}` +
                `&start=${start}&end=${end}` +
                `&type=${typeSel.value}`;
    const res = await fetch(url);
    slots = await res.json();
    document.querySelectorAll('.fc-daygrid-day')
      .forEach(el => el.classList.remove('has-slots'));
    Object.keys(slots).forEach(d => {
      const cell = document.querySelector(`[data-date="${d}"]`);
      if (cell) cell.classList.add('has-slots');
    });
  }

  // 4) выбор даты
  function onDateClick(dateStr, dayEl) {
    if (!slots[dateStr]) return;
    resetDate(); resetTime();
    selDate = dateStr;
    dayEl.classList.add('selected-day');
    renderTimes(dateStr);
    submitBtn.disabled = true;
  }
  function resetDate() {
    document.querySelectorAll('.selected-day')
      .forEach(e => e.classList.remove('selected-day'));
    selDate = null; timesEl.innerHTML = '';
  }

  // 5) рендер слотов
  function renderTimes(date) {
    timesEl.innerHTML = `<h3>Слоты на ${date}</h3>`;
    slots[date].forEach(({time, free}) => {
      const btn = document.createElement('button');
      btn.className   = 'slot-btn';
      btn.textContent = `${time} (${free})`;
      btn.disabled    = free===0;
      btn.onclick     = () => onTimeClick(btn, time);
      timesEl.appendChild(btn);
    });
  }

  // 6) выбор времени
  function onTimeClick(btn, time) {
    if (selBtn === btn) {
      btn.classList.remove('selected-slot');
      selBtn = selTime = null;
      submitBtn.disabled = true;
      return;
    }
    if (selBtn) selBtn.classList.remove('selected-slot');
    btn.classList.add('selected-slot');
    selBtn = btn; selTime = time;
    submitBtn.disabled = !(selDate && selTime);
  }
  function resetTime() {
    if (selBtn) selBtn.classList.remove('selected-slot');
    selBtn = selTime = null; submitBtn.disabled = true;
  }

  // 7) бронирование
  form.onsubmit = async e => {
    e.preventDefault();
    if (!selDate||!selTime) return alert('Выберите дату и время');
    const duration = +typeSel.selectedOptions[0].dataset.duration;
    const body = {
      ServiceCenterId: +centerSel.value,
      ServiceTypeId:   +typeSel.value,
      Date:            selDate,
      Time:            selTime,
      Duration:        duration
    };
    submitBtn.disabled = true;
    const res = await fetch('/api/slots/book',{
      method:'POST',
      headers:{'Content-Type':'application/json'},
      body:JSON.stringify(body)
    });
    if (res.ok) {
      const [h,m] = selTime.split(':').map(Number);
      for(let i=0;i<duration;i++){
        const t = `${String(h+i).padStart(2,'0')}:${String(m).padStart(2,'0')}`;
        const rec = slots[selDate].find(s=>s.time===t);
        if (rec) rec.free--;
        const b = Array.from(timesEl.querySelectorAll('button'))
                       .find(x=>x.textContent.startsWith(t));
        if (b) {
          if (rec.free>0) b.textContent=`${t} (${rec.free})`;
          else b.remove();
        }
      }
      alert('Успешно');
      resetTime();
    } else {
      const err = await res.json().catch(()=>({error:'Ошибка'}));
      alert(err.error);
      if(selBtn) selBtn.remove();
      resetTime();
    }
  };
});



const cardsAuto = document.querySelectorAll('.car-card');
    const nextBtnAuto = document.getElementById('nextBtn');
    let selectedVin = null;
    cardsAuto.forEach(cardAuto => {
      cardAuto.addEventListener('click', () => {
        cardsAuto.forEach(c => c.classList.remove('selected'));
        cardAuto.classList.add('selected');
        selectedVin = cardAuto.dataset.vin;
        nextBtnAuto.disabled = false;
      });
    });
    nextBtnAuto.addEventListener('click', () => {
      console.log('Выбран VIN:', selectedVin);
      // переход на следующий шаг
    });



    const cards = document.querySelectorAll('.service-card');
    const nextBtn = document.getElementById('nextBtn');
    let selectedService = null;
    cards.forEach(card => {
      card.addEventListener('click', () => {
        cards.forEach(c => c.classList.remove('selected'));
        card.classList.add('selected');
        selectedService = card.dataset.value;
        nextBtn.disabled = false;
      });
      card.addEventListener('keypress', e => {
        if (e.key === " " || e.key === "Enter") card.click();
      });
    });
    nextBtn.addEventListener('click', () => {
      console.log('Выбрана услуга:', selectedService);
      // window.location.href = ...
    });




     // Инициализация карты
    //const map = L.map('map').setView([57.1522,65.5272],12);
    //L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    //  attribution: '&copy; OpenStreetMap contributors'
    //}).addTo(map);

   
    // Добавляем маркеры
    document.querySelectorAll('.service-item').forEach(item => {
      const lat = +item.dataset.lat;
      const lng = +item.dataset.lng;
      L.marker([lat, lng]).addTo(map).bindPopup(item.dataset.value || item.querySelector('.service-item__title').innerText);
    });

    // Логика выбора и подсветки
   /* const items = document.querySelectorAll('.service-item');
    const nextBtn = document.getElementById('toPage1-5');
    items.forEach(item => {
      item.addEventListener('click', () => {
        items.forEach(i => i.classList.remove('selected'));
        item.classList.add('selected');
        nextBtn.disabled = false;
        map.flyTo([+item.dataset.lat, +item.dataset.lng], 15);
      });
    });*/

    // Скрываем/показываем шапку и футер при прокрутке
    const scroller = document.getElementById('scroller');
    const hero = document.querySelector('.hero');
    const footer = document.querySelector('.nav-hero');
    let lastY = 0;
    scroller.addEventListener('scroll', () => {
      const y = scroller.scrollTop;
      hero.classList.toggle('hidden', y > lastY);
      footer.classList.toggle('hidden', y > lastY);
      lastY = y;
    });



    