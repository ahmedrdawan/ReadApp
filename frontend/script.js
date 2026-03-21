const books = [
  { title: 'Atomic Habits', author: 'James Clear', cover: 'https://images.unsplash.com/photo-1513001900722-370f803f498d?auto=format&fit=crop&w=500&q=80' },
  { title: 'The Alchemist', author: 'Paulo Coelho', cover: 'https://images.unsplash.com/photo-1530538987395-032d1800fdd4?auto=format&fit=crop&w=500&q=80' },
  { title: 'Deep Work', author: 'Cal Newport', cover: 'https://images.unsplash.com/photo-1521056787327-2f4935e6f9b0?auto=format&fit=crop&w=500&q=80' },
  { title: 'Ikigai', author: 'Héctor García', cover: 'https://images.unsplash.com/photo-1495446815901-a7297e633e8d?auto=format&fit=crop&w=500&q=80' },
  { title: 'The Psychology of Money', author: 'Morgan Housel', cover: 'https://images.unsplash.com/photo-1495640388908-05fa85288e61?auto=format&fit=crop&w=500&q=80' },
  { title: 'Think Again', author: 'Adam Grant', cover: 'https://images.unsplash.com/photo-1519682337058-a94d519337bc?auto=format&fit=crop&w=500&q=80' },
];

const navTemplate = [
  { label: 'Dashboard', href: './dashboard.html' },
  { label: 'Discover', href: './discover.html' },
  { label: 'My Library', href: './library.html' },
  { label: 'Book Details', href: './book-details.html' },
  { label: 'Profile', href: './profile.html' },
  { label: 'Settings', href: './settings.html' },
];

function renderNav() {
  const sidebars = document.querySelectorAll('[data-nav]');
  sidebars.forEach((sidebar) => {
    const page = window.location.pathname.split('/').pop();
    sidebar.innerHTML = `
      <h1 class="brand">Readify</h1>
      <nav>
        ${navTemplate
          .map((item) => {
            const active = page === item.href.replace('./', '') ? 'active' : '';
            return `<a class="nav-item ${active}" href="${item.href}">${item.label}</a>`;
          })
          .join('')}
      </nav>
      <div class="profile-card">
        <img src="https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=120&q=80" alt="Profile" />
        <div>
          <p class="name">Maya Carter</p>
          <p class="subtle">Reading streak: 12 days</p>
        </div>
      </div>
    `;
  });
}

function renderBooks(list) {
  const grid = document.getElementById('bookGrid');
  if (!grid) return;
  grid.innerHTML = '';
  list.forEach((book) => {
    const card = document.createElement('article');
    card.className = 'book-card card';
    card.innerHTML = `
      <img src="${book.cover}" alt="${book.title} cover" />
      <h4>${book.title}</h4>
      <p>${book.author}</p>
    `;
    grid.appendChild(card);
  });
}

function wireSearch() {
  const search = document.getElementById('searchInput');
  if (!search) return;
  search.addEventListener('input', (event) => {
    const query = event.target.value.trim().toLowerCase();
    const filtered = books.filter((book) =>
      `${book.title} ${book.author}`.toLowerCase().includes(query)
    );
    renderBooks(filtered);
  });
}

renderNav();
renderBooks(books);
wireSearch();
