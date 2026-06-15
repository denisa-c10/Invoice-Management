<template>
  <div class="app-wrapper">
    <header class="main-header">
        <div class="logo">
            <i class="fas fa-check-circle"></i> Gestionare Facturi
        </div>
        <h1 class="page-title">Coman Denisa</h1>
    </header>

    <nav class="main-nav">
        <ul>
            <li><router-link to="/"><i class="fas fa-home"></i> Acasă</router-link></li>
            <li><router-link to="/produse"><i class="fas fa-box"></i> Produse</router-link></li>
            <li><router-link to="/clienti"><i class="fas fa-users"></i> Clienți</router-link></li>
            <li><router-link to="/facturi"><i class="fas fa-file-invoice-dollar"></i> Facturi</router-link></li>
        </ul>
    </nav>

    <main class="main-content">
        <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-wrapper {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
}

/* HEADER */
.main-header {
    background-color: #1a202e;
    color: white;
    padding: 15px 30px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.logo {
    font-size: 1.2rem;
    font-weight: bold;
    display: flex;
    align-items: center;
    gap: 10px;
}
.logo i { color: #63b3ed; }
.page-title { font-size: 1.4rem; font-weight: 500; margin: 0;}

/* NAVIGARE */
.main-nav {
    background-color: white;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    padding: 0 30px;
}

.main-nav ul {
    list-style: none;
    display: flex;
    margin: 0;
    padding: 0;
}

.main-nav li {
    margin-right: 5px;
}

.main-nav a {
    text-decoration: none;
    color: #64748b;
    padding: 15px 20px;
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: 500;
    border-bottom: 3px solid transparent; /* Pentru linia de jos cand e activ */
    transition: all 0.2s ease;
}

.main-nav a:hover {
    color: #1e293b;
    background-color: #f8fafc;
}

/* MAGIA VUE ROUTER: Tab-ul activ */
.main-nav a.router-link-active {
    color: #2563eb; /* Albastru aprins */
    border-bottom: 3px solid #2563eb;
    background-color: #eff6ff;
}

/* ZONA UNDE APAR PAGINILE */
.main-content {
    flex: 1;
    padding: 30px;
    background-color: #f4f7f6; /* Gri-ul deschis din spate */
}
</style>