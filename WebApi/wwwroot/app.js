/* =====================================================
   SweetCandy — Frontend SPA
   Integração com a API SweetCandy (.NET 8)
   ===================================================== */

const API = '/api';

// ── Helpers de estado global ──────────────────────────
let currentPage = 'dashboard';
const state = { page: 1, pageSize: 20 };

// ── Estado do Dashboard ───────────────────────────────
const now = new Date();
let dashMonth = now.getMonth();   // 0-11
let dashYear  = now.getFullYear();
let valuesHidden = false;

const MESES = ['Janeiro','Fevereiro','Março','Abril','Maio','Junho',
               'Julho','Agosto','Setembro','Outubro','Novembro','Dezembro'];

// ── Mapeamentos de enum ───────────────────────────────
const STATUS_ORDER = { 0: 'Pendente', 1: 'Confirmada', 2: 'Cancelada', 3: 'Concluída' };
const STATUS_CLASS  = { 0: 's-0', 1: 's-1', 2: 's-2', 3: 's-3' };
const FORMA_PAG    = { 0: 'Dinheiro', 1: 'Débito', 2: 'Crédito', 3: 'Pix' };
const UNIDADE      = { 0: 'Un', 1: 'Kg', 2: 'G', 3: 'L', 4: 'Ml', 5: 'Mg', 6: 'Cx', 7: 'Pct' };
const MOV_TYPE     = { 0: 'Saída', 1: 'Entrada' };

// ── Inicialização ─────────────────────────────────────
window.addEventListener('DOMContentLoaded', () => {
  // Seta data no topbar
  document.getElementById('topbar-date').textContent =
    new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' });

  // Registra cliques da sidebar
  document.querySelectorAll('.nav-item').forEach(el => {
    el.addEventListener('click', () => navigate(el.dataset.page));
  });

  navigate('dashboard');
});

// ── Navegação ─────────────────────────────────────────
function navigate(page) {
  currentPage = page;
  document.querySelectorAll('.nav-item').forEach(el => el.classList.toggle('active', el.dataset.page === page));
  const titles = {
    dashboard: 'Painel', financas: 'Finanças', pedidos: 'Pedidos',
    estoque: 'Estoque', orcamentos: 'Orçamentos', clientes: 'Clientes',
    categorias: 'Categorias', recibos: 'Recibos', despesas: 'Despesas',
    movimentacoes: 'Movimentações de Estoque'
  };
  document.getElementById('topbar-title').textContent = titles[page] || page;
  const renders = {
    dashboard: renderDashboard,
    financas: renderFinancas,
    pedidos: renderPedidos,
    estoque: renderEstoque,
    orcamentos: renderOrcamentos,
    clientes: () => renderCrud('clientes'),
    categorias: () => renderCrud('categorias'),
    recibos: renderRecibos,
    despesas: renderDespesas,
    movimentacoes: renderMovimentacoes,
  };
  (renders[page] || (() => {}))();
}

function toggleSidebar() {
  const sb = document.getElementById('sidebar');
  if (window.innerWidth <= 768) sb.classList.toggle('open');
  else sb.classList.toggle('collapsed');
}

// ── API ────────────────────────────────────────────────
async function api(method, path, body) {
  const opts = {
    method,
    headers: { 'Content-Type': 'application/json' },
  };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(API + path, opts);
  const json = await res.json().catch(() => ({ isSuccessful: false, messages: ['Erro de resposta'] }));
  return json;
}

const get  = (path)         => api('GET', path);
const post = (path, body)   => api('POST', path, body);
const put  = (path, body)   => api('PUT', path, body);
const del  = (path)         => api('DELETE', path);

// ── Toast ─────────────────────────────────────────────
function toast(msg, type = 'success') {
  const t = document.getElementById('toast');
  t.textContent = msg;
  t.className = `toast ${type} show`;
  clearTimeout(t._timer);
  t._timer = setTimeout(() => t.classList.remove('show'), 3000);
}

// ── Modal ─────────────────────────────────────────────
function openModal(title, bodyHtml) {
  document.getElementById('modal-title').textContent = title;
  document.getElementById('modal-body').innerHTML = bodyHtml;
  document.getElementById('modal').classList.add('open');
  document.getElementById('modal-overlay').classList.add('open');
}
function closeModal() {
  document.getElementById('modal').classList.remove('open');
  document.getElementById('modal-overlay').classList.remove('open');
}

// ── Loader ────────────────────────────────────────────
function setContent(html) { document.getElementById('content').innerHTML = html; }
function loadingHtml() { return `<div class="loading"><div class="spinner"></div><br>Carregando...</div>`; }

// ── Formata moeda ─────────────────────────────────────
const brl = v => v != null ? `R$ ${Number(v).toFixed(2).replace('.', ',').replace(/\B(?=(\d{3})+(?!\d))/g, '.')}` : '—';
const brlH = v => valuesHidden ? 'R$ ••••' : brl(v);
const fmtDate = s => s ? new Date(s).toLocaleDateString('pt-BR') : '—';
const fmtDatetime = s => s ? new Date(s).toLocaleString('pt-BR') : '—';
const trunc = (s, n = 30) => s && s.length > n ? s.slice(0, n) + '…' : (s || '—');

// ═══════════════════════════════════════════════════════
//  DASHBOARD
// ═══════════════════════════════════════════════════════

function changeMonth(dir) {
  dashMonth += dir;
  if (dashMonth < 0)  { dashMonth = 11; dashYear--; }
  if (dashMonth > 11) { dashMonth = 0;  dashYear++; }
  renderDashboard();
}

function toggleValues() {
  valuesHidden = !valuesHidden;
  renderDashboard();
}

async function renderDashboard() {
  setContent(loadingHtml());

  const [receipts, expenses, orders, customers] = await Promise.all([
    get('/Receipts/GetAll?page=1&pageSize=1000'),
    get('/Expenses/GetAll?page=1&pageSize=1000'),
    get('/Orders/GetAll?page=1&pageSize=1000'),
    get('/Customers/GetAll?page=1&pageSize=1000'),
  ]);

  const totalReceitas = (receipts.data || []).reduce((s, r) => s + (r.amount || 0), 0);
  const totalDespesas = (expenses.data || []).reduce((s, e) => s + (e.value || 0), 0);
  const pedidosPendentes = (orders.data || []).filter(o => o.status === 0).length;
  const pedidosConfirmados = (orders.data || []).filter(o => o.status === 1).length;
  const totalClientes = (customers.data || []).length;
  const lucro = totalReceitas - totalDespesas;
  const metaMensal = 5000;
  const metaPct = Math.min(100, Math.round((totalReceitas / metaMensal) * 100));
  const eyeIcon = valuesHidden
    ? '<i class="fa-solid fa-eye-slash"></i> Exibir Valores'
    : '<i class="fa-solid fa-eye"></i> Esconder Valores';

  setContent(`
    <section class="welcome-section">
      <p class="greeting">Bem-vinda! 👋</p>
      <h1>Olá, SweetCandy!</h1>
    </section>

    <div class="month-nav">
      <button class="month-nav-btn" onclick="changeMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
      <span class="month-nav-label">${MESES[dashMonth]} de ${dashYear}</span>
      <button class="month-nav-btn" onclick="changeMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
      <button class="hide-values-btn" onclick="toggleValues()">${eyeIcon}</button>
    </div>

    <div class="summary-cards">
      <div class="summary-card" onclick="navigate('recibos')" style="cursor:pointer">
        <div class="sc-icon green"><i class="fa-solid fa-arrow-trend-up"></i></div>
        <div class="sc-body">
          <div class="sc-label">Receitas</div>
          <div class="sc-value green">${brlH(totalReceitas)}</div>
        </div>
      </div>
      <div class="summary-card" onclick="navigate('despesas')" style="cursor:pointer">
        <div class="sc-icon red"><i class="fa-solid fa-arrow-trend-down"></i></div>
        <div class="sc-body">
          <div class="sc-label">Despesas</div>
          <div class="sc-value red">${brlH(totalDespesas)}</div>
        </div>
      </div>
      <div class="summary-card">
        <div class="sc-icon orange"><i class="fa-solid fa-scale-balanced"></i></div>
        <div class="sc-body">
          <div class="sc-label">Lucro</div>
          <div class="sc-value ${lucro >= 0 ? 'green' : 'red'}">${brlH(lucro)}</div>
        </div>
      </div>
    </div>

    <div class="meta-card">
      <div class="meta-card-header">
        <span class="meta-card-title"><i class="fa-solid fa-bullseye"></i> Meta Mensal</span>
        <span class="meta-pct">${metaPct}%</span>
      </div>
      <div class="progress-track">
        <div class="progress-fill" style="width:${metaPct}%"></div>
      </div>
      <div class="meta-details">
        <div class="meta-detail">
          <span class="meta-detail-label">Realizado</span>
          <span class="meta-detail-value">${brlH(totalReceitas)}</span>
        </div>
        <div class="meta-detail">
          <span class="meta-detail-label">Meta</span>
          <span class="meta-detail-value">${brl(metaMensal)}</span>
        </div>
        <div class="meta-detail">
          <span class="meta-detail-label">Falta</span>
          <span class="meta-detail-value">${brlH(Math.max(0, metaMensal - totalReceitas))}</span>
        </div>
      </div>
    </div>

    <div class="cards-grid">
      <div class="card" onclick="navigate('pedidos')" style="cursor:pointer">
        <div class="card-badge">${pedidosPendentes}</div>
        <div class="card-icon"><i class="fa-solid fa-clipboard-list" style="color:var(--primary)"></i></div>
        <div class="card-label">Pedidos Pendentes</div>
        <div class="card-value pink">${pedidosPendentes}</div>
        <div class="card-sub">${pedidosConfirmados} confirmados</div>
      </div>
      <div class="card" onclick="navigate('clientes')" style="cursor:pointer">
        <div class="card-icon"><i class="fa-solid fa-users" style="color:var(--primary-dark)"></i></div>
        <div class="card-label">Clientes</div>
        <div class="card-value purple">${totalClientes}</div>
        <div class="card-sub">cadastrados</div>
      </div>
    </div>

    <section class="quick-actions">
      <div class="section-header"><h2>Ações rápidas</h2></div>
      <div class="quick-grid">
        <a class="quick-card" onclick="navigate('recibos'); setTimeout(()=>btnNewRecibo(),300)">
          <span class="qc-icon"><i class="fa-solid fa-receipt"></i></span>Adicionar recibo
        </a>
        <a class="quick-card" onclick="navigate('despesas'); setTimeout(()=>btnNewDespesa(),300)">
          <span class="qc-icon"><i class="fa-solid fa-arrow-trend-down"></i></span>Adicionar despesa
        </a>
        <a class="quick-card" onclick="navigate('pedidos'); setTimeout(()=>btnNewPedido(),300)">
          <span class="qc-icon"><i class="fa-solid fa-clipboard-list"></i></span>Novo pedido
        </a>
        <a class="quick-card" onclick="navigate('orcamentos'); setTimeout(()=>btnNewOrcamento(),300)">
          <span class="qc-icon"><i class="fa-solid fa-file-invoice"></i></span>Novo orçamento
        </a>
        <a class="quick-card" onclick="navigate('clientes'); setTimeout(()=>btnNewCliente(),300)">
          <span class="qc-icon"><i class="fa-solid fa-user-plus"></i></span>Novo cliente
        </a>
        <a class="quick-card" onclick="navigate('estoque')">
          <span class="qc-icon"><i class="fa-solid fa-boxes-stacked"></i></span>Ver estoque
        </a>
      </div>
    </section>

    ${(orders.data||[]).length > 0 ? `
    <section>
      <div class="section-header"><h2>Pedidos recentes</h2></div>
      <div class="table-wrap">
        <table class="data-table">
          <thead><tr><th>Pedido</th><th>Data</th><th>Total</th><th>Status</th></tr></thead>
          <tbody>
            ${(orders.data||[]).slice(-5).reverse().map(o => `
              <tr>
                <td><strong>${o.name || '—'}</strong></td>
                <td>${fmtDate(o.eventDate)}</td>
                <td>${brl(o.totalValue)}</td>
                <td><span class="badge ${STATUS_CLASS[o.status]}">${STATUS_ORDER[o.status]}</span></td>
              </tr>
            `).join('')}
          </tbody>
        </table>
      </div>
    </section>` : ''}
  `);
}

// ═══════════════════════════════════════════════════════
//  FINANÇAS (Receitas + Despesas combinadas)
// ═══════════════════════════════════════════════════════
async function renderFinancas() {
  setContent(loadingHtml());
  const [receipts, expenses] = await Promise.all([
    get('/Receipts/GetAll?page=1&pageSize=1000'),
    get('/Expenses/GetAll?page=1&pageSize=1000'),
  ]);
  const rList = receipts.data || [];
  const eList = expenses.data || [];
  const totalR = rList.reduce((s, r) => s + (r.amount || 0), 0);
  const totalE = eList.reduce((s, e) => s + (e.value || 0), 0);

  setContent(`
    <div class="page-header"><h1><i class="fa-solid fa-wallet" style="color:var(--primary);margin-right:8px"></i>Finanças</h1></div>
    <div class="cards-grid" style="margin-bottom:24px">
      <div class="card">
        <div class="card-badge">${rList.length}</div>
        <div class="card-icon"><i class="fa-solid fa-arrow-trend-up" style="color:var(--success)"></i></div>
        <div class="card-label">Receitas</div>
        <div class="card-value green">${brl(totalR)}</div>
      </div>
      <div class="card">
        <div class="card-badge">${eList.length}</div>
        <div class="card-icon"><i class="fa-solid fa-arrow-trend-down" style="color:var(--danger)"></i></div>
        <div class="card-label">Despesas</div>
        <div class="card-value red">${brl(totalE)}</div>
      </div>
      <div class="card">
        <div class="card-icon"><i class="fa-solid fa-scale-balanced" style="color:var(--primary)"></i></div>
        <div class="card-label">Saldo</div>
        <div class="card-value ${totalR - totalE >= 0 ? 'green' : 'red'}">${brl(totalR - totalE)}</div>
      </div>
    </div>

    <div class="tabs">
      <button class="tab-btn active" id="tab-todas" onclick="finTab('todas')">Todas</button>
      <button class="tab-btn" id="tab-receitas" onclick="finTab('receitas')">Receitas</button>
      <button class="tab-btn" id="tab-despesas" onclick="finTab('despesas')">Despesas</button>
    </div>

    <div id="fin-list">
      ${buildFinList(rList, eList, 'todas')}
    </div>
  `);
}

function finTab(tab) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('tab-' + tab).classList.add('active');

  // re-busca os dados já carregados do DOM seria complexo, então re-renderiza via API
  Promise.all([
    get('/Receipts/GetAll?page=1&pageSize=1000'),
    get('/Expenses/GetAll?page=1&pageSize=1000'),
  ]).then(([r, e]) => {
    document.getElementById('fin-list').innerHTML = buildFinList(r.data || [], e.data || [], tab);
  });
}

function buildFinList(receipts, expenses, tab) {
  let html = '';
  if (tab !== 'receitas' && expenses.length > 0) {
    html += `<div class="tx-group">
      <div class="tx-group-header"><span>Despesas</span><span>${brl(expenses.reduce((s,e)=>s+(e.value||0),0))}</span></div>
      ${expenses.map(e => `
        <div class="tx-item">
          <div class="tx-icon despesa"><i class="fa-solid fa-arrow-trend-down"></i></div>
          <div class="tx-info">
            <strong>${e.name || '—'}</strong>
            <span>${e.paid ? '<i class="fa-solid fa-check" style="color:var(--success)"></i> Pago' : '<i class="fa-regular fa-clock" style="color:var(--warning)"></i> Pendente'}</span>
          </div>
          <div class="tx-value despesa">− ${brl(e.value)}</div>
        </div>`).join('')}
    </div>`;
  }
  if (tab !== 'despesas' && receipts.length > 0) {
    html += `<div class="tx-group">
      <div class="tx-group-header"><span>Receitas</span><span>${brl(receipts.reduce((s,r)=>s+(r.amount||0),0))}</span></div>
      ${receipts.map(r => `
        <div class="tx-item">
          <div class="tx-icon receita"><i class="fa-solid fa-arrow-trend-up"></i></div>
          <div class="tx-info">
            <strong>${r.finalProductName || 'Recibo'}</strong>
            <span>${fmtDate(r.date)} · ${FORMA_PAG[r.paymentMethod] || '—'}</span>
          </div>
          <div class="tx-value receita">+ ${brl(r.amount)}</div>
        </div>`).join('')}
    </div>`;
  }
  return html || `<div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-chart-bar"></i></div><p>Nenhum registro</p></div>`;
}

// ═══════════════════════════════════════════════════════
//  PEDIDOS
// ═══════════════════════════════════════════════════════
let pedidosData = [];

async function renderPedidos() {
  setContent(loadingHtml());
  const res = await get('/Orders/GetAll?page=1&pageSize=1000');
  pedidosData = res.data || [];
  buildPedidosTable(pedidosData);
}

function buildPedidosTable(list) {
  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-clipboard-list" style="color:var(--primary);margin-right:8px"></i>Pedidos</h1>
      <button class="btn btn-primary" onclick="btnNewPedido()"><i class="fa-solid fa-plus"></i> Novo Pedido</button>
    </div>
    <div class="filters">
      <input class="search-input" placeholder="Buscar pedido..." oninput="filterPedidos(this.value)" />
      <select class="form-control" style="width:auto" onchange="filterPedidoStatus(this.value)">
        <option value="">Todos os status</option>
        <option value="0">Pendente</option>
        <option value="1">Confirmada</option>
        <option value="2">Cancelada</option>
        <option value="3">Concluída</option>
      </select>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Pedido</th><th>Data</th><th>Sinal</th><th>Total</th><th>Status</th><th>Ações</th></tr></thead>
        <tbody id="pedidos-body">
          ${list.length === 0 ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-clipboard-list"></i></div><p>Nenhum pedido</p></div></td></tr>` :
            list.map(o => `
              <tr>
                <td><strong>${o.name || '—'}</strong></td>
                <td>${fmtDate(o.eventDate)}</td>
                <td>${brl(o.sinal)}</td>
                <td>${brl(o.totalValue)}</td>
                <td><span class="badge ${STATUS_CLASS[o.status]}">${STATUS_ORDER[o.status]}</span></td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='viewPedido(${JSON.stringify(o)})' title="Ver"><i class="fa-solid fa-eye"></i></button>
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editPedido(${JSON.stringify(o)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deletePedido('${o.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function filterPedidos(q) {
  const f = pedidosData.filter(o => (o.name||'').toLowerCase().includes(q.toLowerCase()));
  document.getElementById('pedidos-body').innerHTML = f.map(o => `
    <tr>
      <td><strong>${o.name||'—'}</strong></td>
      <td>${fmtDate(o.eventDate)}</td>
      <td>${brl(o.sinal)}</td>
      <td>${brl(o.totalValue)}</td>
      <td><span class="badge ${STATUS_CLASS[o.status]}">${STATUS_ORDER[o.status]}</span></td>
      <td><div class="actions">
        <button class="btn btn-sm btn-secondary btn-icon" onclick='viewPedido(${JSON.stringify(o)})'><i class="fa-solid fa-eye"></i></button>
        <button class="btn btn-sm btn-secondary btn-icon" onclick='editPedido(${JSON.stringify(o)})'><i class="fa-solid fa-pencil"></i></button>
        <button class="btn btn-sm btn-danger btn-icon" onclick="deletePedido('${o.id}')"><i class="fa-solid fa-trash"></i></button>
      </div></td>
    </tr>`).join('') || `<tr><td colspan="6"><div class="empty-state"><p>Nenhum resultado</p></div></td></tr>`;
}

function filterPedidoStatus(v) {
  const f = v === '' ? pedidosData : pedidosData.filter(o => String(o.status) === v);
  buildPedidosTable(f);
}

function viewPedido(o) {
  const items = (o.items||[]).map(i => `
    <div class="item-row">
      <span>${i.finalProductName||'—'} × ${i.quantity}</span>
      <span>${brl(i.totalPrice)}</span>
    </div>`).join('') || '<p style="color:var(--text-muted);font-size:.85rem">Sem itens</p>';
  openModal('Detalhes do Pedido', `
    <div class="detail-grid" style="margin-bottom:16px">
      <div class="detail-item"><label>Nome</label><span>${o.name||'—'}</span></div>
      <div class="detail-item"><label>Status</label><span class="badge ${STATUS_CLASS[o.status]}">${STATUS_ORDER[o.status]}</span></div>
      <div class="detail-item"><label>Data Evento</label><span>${fmtDate(o.eventDate)}</span></div>
      <div class="detail-item"><label>Sinal</label><span>${brl(o.sinal)}</span></div>
      <div class="detail-item"><label>Total</label><span>${brl(o.totalValue)}</span></div>
    </div>
    <h3 style="font-size:.9rem;margin-bottom:8px">Itens</h3>
    <div class="items-list">${items}</div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Fechar</button>
      <button class="btn btn-primary" onclick="closeModal();editPedido(${JSON.stringify(o).replace(/"/g,'&quot;')})">Editar</button>
    </div>
  `);
}

function btnNewPedido() {
  openModal('Novo Pedido', formPedido(null));
}

function editPedido(o) {
  openModal('Editar Pedido', formPedido(o));
}

function formPedido(o) {
  const statusOpts = Object.entries(STATUS_ORDER).map(([k,v]) =>
    `<option value="${k}" ${o && o.status == k ? 'selected' : ''}>${v}</option>`).join('');
  const pagOpts = Object.entries(FORMA_PAG).map(([k,v]) =>
    `<option value="${k}">${v}</option>`).join('');
  return `
    <div class="form-grid">
      <div class="form-group">
        <label>Nome do Pedido *</label>
        <input class="form-control" id="p-name" value="${o?.name||''}" placeholder="Ex: Pedido Casamento Maria" required />
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Data do Evento</label>
          <input class="form-control" id="p-date" type="date" value="${o?.eventDate ? o.eventDate.slice(0,10) : ''}" />
        </div>
        <div class="form-group">
          <label>Status</label>
          <select class="form-control" id="p-status">${statusOpts}</select>
        </div>
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Sinal (R$)</label>
          <input class="form-control" id="p-sinal" type="number" step="0.01" value="${o?.sinal||''}" placeholder="0,00" />
        </div>
        <div class="form-group">
          <label>Total (R$)</label>
          <input class="form-control" id="p-total" type="number" step="0.01" value="${o?.totalValue||''}" placeholder="0,00" />
        </div>
      </div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${o ? `savePedido('${o.id}')` : 'createPedido()'}">Salvar</button>
    </div>
  `;
}

async function createPedido() {
  const body = {
    name: document.getElementById('p-name').value,
    eventDate: document.getElementById('p-date').value || null,
    status: parseInt(document.getElementById('p-status').value),
    sinal: parseFloat(document.getElementById('p-sinal').value) || null,
    totalValue: parseFloat(document.getElementById('p-total').value) || null,
    items: []
  };
  if (!body.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Orders/Create', body);
  if (r.isSuccessful) { closeModal(); toast('Pedido criado!'); renderPedidos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function savePedido(id) {
  const body = {
    name: document.getElementById('p-name').value || null,
    eventDate: document.getElementById('p-date').value || null,
    status: parseInt(document.getElementById('p-status').value),
    sinal: parseFloat(document.getElementById('p-sinal').value) || null,
    totalValue: parseFloat(document.getElementById('p-total').value) || null,
  };
  const r = await put(`/Orders/Update/${id}`, body);
  if (r.isSuccessful) { closeModal(); toast('Pedido atualizado!'); renderPedidos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function deletePedido(id) {
  if (!confirm('Excluir este pedido?')) return;
  const r = await del(`/Orders/Delete/${id}`);
  if (r.isSuccessful) { toast('Pedido excluído!'); renderPedidos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  ORÇAMENTOS
// ═══════════════════════════════════════════════════════
let orcamentosData = [];

async function renderOrcamentos() {
  setContent(loadingHtml());
  const res = await get('/Budgets/GetAll?page=1&pageSize=1000');
  orcamentosData = res.data || [];
  buildOrcamentosTable(orcamentosData);
}

function buildOrcamentosTable(list) {
  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-file-invoice" style="color:var(--primary);margin-right:8px"></i>Orçamentos</h1>
      <button class="btn btn-primary" onclick="btnNewOrcamento()">+ Novo Orçamento</button>
    </div>
    <div class="filters">
      <input class="search-input" placeholder="Buscar orçamento..." oninput="filterOrcamentos(this.value)" />
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Cliente</th><th>Produto</th><th>Data Evento</th><th>Total</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="5"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-file-invoice"></i></div><p>Nenhum orçamento</p></div></td></tr>` :
            list.map(b => `
              <tr>
                <td><strong>${b.clientName||'—'}</strong></td>
                <td>${trunc(b.finalProductName)}</td>
                <td>${fmtDate(b.eventDate)}</td>
                <td>${brl(b.finalTotalValue)}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='viewOrcamento(${JSON.stringify(b)})' title="Ver"><i class="fa-solid fa-eye"></i></button>
                  <button class="btn btn-sm btn-success btn-icon" onclick="convertToOrder('${b.id}')" title="Converter em Pedido"><i class="fa-solid fa-arrows-rotate"></i></button>
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editOrcamento(${JSON.stringify(b)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteOrcamento('${b.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function filterOrcamentos(q) { renderOrcamentos(); }

function viewOrcamento(b) {
  const items = (b.items||[]).map(i => `
    <div class="item-row">
      <span>${i.finalProductName||'—'} × ${i.quantity}</span>
      <span>${brl(i.totalPrice)}</span>
    </div>`).join('');
  openModal('Detalhes do Orçamento', `
    <div class="detail-grid" style="margin-bottom:16px">
      <div class="detail-item"><label>Cliente</label><span>${b.clientName||'—'}</span></div>
      <div class="detail-item"><label>Data Evento</label><span>${fmtDate(b.eventDate)}</span></div>
      <div class="detail-item"><label>Produto</label><span>${b.finalProductName||'—'}</span></div>
      <div class="detail-item"><label>Total</label><span>${brl(b.finalTotalValue)}</span></div>
    </div>
    <h3 style="font-size:.9rem;margin-bottom:8px">Itens</h3>
    <div class="items-list">${items||'<p style="color:var(--text-muted)">Sem itens</p>'}</div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Fechar</button>
      <button class="btn btn-success" onclick="closeModal();convertToOrder('${b.id}')"><i class="fa-solid fa-arrows-rotate"></i> Converter em Pedido</button>
    </div>
  `);
}

function btnNewOrcamento() { openModal('Novo Orçamento', formOrcamento(null)); }
function editOrcamento(b)  { openModal('Editar Orçamento', formOrcamento(b)); }

function formOrcamento(b) {
  return `
    <div class="form-grid">
      <div class="form-row">
        <div class="form-group">
          <label>Nome do Cliente</label>
          <input class="form-control" id="b-client" value="${b?.clientName||''}" placeholder="Ex: Maria Silva" />
        </div>
        <div class="form-group">
          <label>Data do Evento</label>
          <input class="form-control" id="b-date" type="date" value="${b?.eventDate ? b.eventDate.slice(0,10) : ''}" />
        </div>
      </div>
      <div class="form-group">
        <label>Produto Principal</label>
        <input class="form-control" id="b-prod" value="${b?.finalProductName||''}" placeholder="Ex: Bolo de Casamento" />
      </div>
      <div class="form-group">
        <label>Descrição</label>
        <input class="form-control" id="b-desc" value="${b?.finalProductDescription||''}" placeholder="Detalhes do produto" />
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Preço Unitário (R$)</label>
          <input class="form-control" id="b-unit" type="number" step="0.01" value="${b?.finalUnitPrice||''}" placeholder="0,00" />
        </div>
        <div class="form-group">
          <label>Total (R$)</label>
          <input class="form-control" id="b-total" type="number" step="0.01" value="${b?.finalTotalValue||''}" placeholder="0,00" />
        </div>
      </div>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${b ? `saveOrcamento('${b.id}')` : 'createOrcamento()'}">Salvar</button>
    </div>
  `;
}

async function createOrcamento() {
  const body = {
    clientName: document.getElementById('b-client').value || null,
    eventDate: document.getElementById('b-date').value || null,
    finalProductName: document.getElementById('b-prod').value || null,
    finalProductDescription: document.getElementById('b-desc').value || null,
    finalUnitPrice: parseFloat(document.getElementById('b-unit').value) || null,
    finalTotalValue: parseFloat(document.getElementById('b-total').value) || null,
    items: []
  };
  const r = await post('/Budgets/Create', body);
  if (r.isSuccessful) { closeModal(); toast('Orçamento criado!'); renderOrcamentos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function saveOrcamento(id) {
  const body = {
    clientName: document.getElementById('b-client').value || null,
    eventDate: document.getElementById('b-date').value || null,
    finalProductName: document.getElementById('b-prod').value || null,
    finalProductDescription: document.getElementById('b-desc').value || null,
    finalUnitPrice: parseFloat(document.getElementById('b-unit').value) || null,
    finalTotalValue: parseFloat(document.getElementById('b-total').value) || null,
  };
  const r = await put(`/Budgets/Update/${id}`, body);
  if (r.isSuccessful) { closeModal(); toast('Orçamento atualizado!'); renderOrcamentos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function deleteOrcamento(id) {
  if (!confirm('Excluir este orçamento?')) return;
  const r = await del(`/Budgets/Delete/${id}`);
  if (r.isSuccessful) { toast('Orçamento excluído!'); renderOrcamentos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function convertToOrder(id) {
  if (!confirm('Converter este orçamento em pedido?')) return;
  const r = await post(`/Budgets/ConvertToOrder/${id}`);
  if (r.isSuccessful) { toast('Convertido em pedido! ✅'); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  CLIENTES — CRUD Genérico
// ═══════════════════════════════════════════════════════
async function renderCrud(module) {
  const configs = {
    clientes: {
      title: 'Clientes',
      icon: '<i class="fa-solid fa-users"></i>',
      endpoint: 'Customers',
      cols: ['Nome', 'Email', 'Telefone', 'Endereço'],
      row: c => [c.name||'—', c.email||'—', c.phone||'—', trunc(c.address, 25)],
      newLabel: 'Novo Cliente',
      form: formCliente,
      newFn: 'btnNewCliente',
      editFn: 'editCliente',
      deleteFn: 'deleteCliente',
    },
    categorias: {
      title: 'Categorias',
      icon: '<i class="fa-solid fa-tags"></i>',
      endpoint: 'Categories',
      cols: ['Nome', 'Descrição'],
      row: c => [c.name||'—', trunc(c.description, 40)],
      newLabel: 'Nova Categoria',
      form: formCategoria,
      newFn: 'btnNewCategoria',
      editFn: 'editCategoria',
      deleteFn: 'deleteCategoria',
    },
  };
  const cfg = configs[module];
  if (!cfg) return;
  setContent(loadingHtml());
  const res = await get(`/${cfg.endpoint}/GetAll?page=1&pageSize=1000`);
  const list = res.data || [];
  setContent(`
    <div class="page-header">
      <h1>${cfg.title}</h1>
      <button class="btn btn-primary" onclick="${cfg.newFn}()">${cfg.icon} ${cfg.newLabel}</button>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr>${cfg.cols.map(c=>`<th>${c}</th>`).join('')}<th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="${cfg.cols.length+1}"><div class="empty-state"><div class="empty-icon">${cfg.icon}</div><p>Nenhum registro</p></div></td></tr>` :
            list.map(item => `
              <tr>
                ${cfg.row(item).map(v=>`<td>${v}</td>`).join('')}
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='${cfg.editFn}(${JSON.stringify(item)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="${cfg.deleteFn}('${item.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

// --- Clientes ---
function btnNewCliente()  { openModal('Novo Cliente', formCliente(null)); }
function editCliente(c)   { openModal('Editar Cliente', formCliente(c)); }
function formCliente(c) {
  return `<div class="form-grid">
    <div class="form-row">
      <div class="form-group"><label>Nome *</label><input class="form-control" id="c-name" value="${c?.name||''}" required /></div>
      <div class="form-group"><label>Telefone</label><input class="form-control" id="c-phone" value="${c?.phone||''}" placeholder="(11) 99999-0000" /></div>
    </div>
    <div class="form-group"><label>Email</label><input class="form-control" id="c-email" type="email" value="${c?.email||''}" /></div>
    <div class="form-group"><label>Endereço</label><input class="form-control" id="c-addr" value="${c?.address||''}" /></div>
    <div class="form-group"><label>Nascimento</label><input class="form-control" id="c-birth" type="date" value="${c?.birthDate ? c.birthDate.slice(0,10) : ''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${c ? `saveCliente('${c.id}')` : 'createCliente()'}">Salvar</button>
    </div>
  </div>`;
}
async function createCliente() {
  const b = { name: v('c-name'), phone: v('c-phone')||null, email: v('c-email')||null, address: v('c-addr')||null, birthDate: v('c-birth')||null };
  if (!b.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Customers/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Cliente criado!'); renderCrud('clientes'); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveCliente(id) {
  const b = { name: v('c-name')||null, phone: v('c-phone')||null, email: v('c-email')||null, address: v('c-addr')||null, birthDate: v('c-birth')||null };
  const r = await put(`/Customers/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Cliente atualizado!'); renderCrud('clientes'); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteCliente(id) {
  if (!confirm('Excluir este cliente?')) return;
  const r = await del(`/Customers/Delete/${id}`);
  if (r.isSuccessful) { toast('Cliente excluído!'); renderCrud('clientes'); }
  else toast((r.messages||[]).join(', '), 'error');
}

// --- Categorias ---
function btnNewCategoria() { openModal('Nova Categoria', formCategoria(null)); }
function editCategoria(c)  { openModal('Editar Categoria', formCategoria(c)); }
function formCategoria(c) {
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label><input class="form-control" id="cat-name" value="${c?.name||''}" required /></div>
    <div class="form-group"><label>Descrição</label><textarea class="form-control" id="cat-desc" rows="2">${c?.description||''}</textarea></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${c ? `saveCategoria('${c.id}')` : 'createCategoria()'}">Salvar</button>
    </div>
  </div>`;
}
async function createCategoria() {
  const b = { name: v('cat-name'), description: v('cat-desc')||null };
  if (!b.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Categories/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Categoria criada!'); renderCrud('categorias'); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveCategoria(id) {
  const b = { name: v('cat-name')||null, description: v('cat-desc')||null };
  const r = await put(`/Categories/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Categoria atualizada!'); renderCrud('categorias'); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteCategoria(id) {
  if (!confirm('Excluir esta categoria?')) return;
  const r = await del(`/Categories/Delete/${id}`);
  if (r.isSuccessful) { toast('Categoria excluída!'); renderCrud('categorias'); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  DESPESAS
// ═══════════════════════════════════════════════════════
async function renderDespesas() {
  setContent(loadingHtml());
  const res = await get('/Expenses/GetAll?page=1&pageSize=1000');
  const list = res.data || [];
  const total = list.reduce((s, e) => s + (e.value || 0), 0);
  const pagas = list.filter(e => e.paid).reduce((s,e)=>s+(e.value||0),0);
  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-arrow-trend-down" style="color:var(--primary);margin-right:8px"></i>Despesas</h1>
      <button class="btn btn-primary" onclick="btnNewDespesa()"><i class="fa-solid fa-plus"></i> Nova Despesa</button>
    </div>
    <div class="cards-grid" style="margin-bottom:20px">
      <div class="card"><div class="card-icon"><i class="fa-solid fa-arrow-trend-down" style="color:var(--danger)"></i></div><div class="card-label">Total</div><div class="card-value red">${brl(total)}</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-circle-check" style="color:var(--success)"></i></div><div class="card-label">Pago</div><div class="card-value green">${brl(pagas)}</div></div>
      <div class="card"><div class="card-icon"><i class="fa-regular fa-clock" style="color:var(--primary)"></i></div><div class="card-label">Pendente</div><div class="card-value pink">${brl(total - pagas)}</div></div>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Nome</th><th>Valor</th><th>Status</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="4"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-arrow-trend-down"></i></div><p>Nenhuma despesa</p></div></td></tr>` :
            list.map(e => `
              <tr>
                <td><strong>${e.name||'—'}</strong></td>
                <td>${brl(e.value)}</td>
                <td><span class="badge ${e.paid ? 'badge-green' : 'badge-yellow'}">${e.paid ? 'Pago' : 'Pendente'}</span></td>
                <td><div class="actions">
                  ${!e.paid ? `<button class="btn btn-sm btn-success btn-icon" onclick="markDespesaPaga('${e.id}')" title="Marcar como pago"><i class="fa-solid fa-check"></i></button>` : ''}
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editDespesa(${JSON.stringify(e)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteDespesa('${e.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function btnNewDespesa()  { openModal('Nova Despesa', formDespesa(null)); }
function editDespesa(e)   { openModal('Editar Despesa', formDespesa(e)); }
function formDespesa(e) {
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label><input class="form-control" id="e-name" value="${e?.name||''}" placeholder="Ex: Conta de Luz" required /></div>
    <div class="form-group"><label>Valor (R$) *</label><input class="form-control" id="e-val" type="number" step="0.01" value="${e?.value||''}" placeholder="0,00" required /></div>
    <div class="form-group">
      <label>Status</label>
      <select class="form-control" id="e-paid">
        <option value="false" ${!e?.paid ? 'selected' : ''}>Pendente</option>
        <option value="true"  ${e?.paid ? 'selected' : ''}>Pago</option>
      </select>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${e ? `saveDespesa('${e.id}')` : 'createDespesa()'}">Salvar</button>
    </div>
  </div>`;
}
async function createDespesa() {
  const b = { name: v('e-name'), value: parseFloat(v('e-val'))||0, paid: v('e-paid') === 'true' };
  if (!b.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Expenses/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Despesa criada!'); renderDespesas(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveDespesa(id) {
  const b = { name: v('e-name')||null, value: parseFloat(v('e-val'))||null, paid: v('e-paid') === 'true' };
  const r = await put(`/Expenses/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Despesa atualizada!'); renderDespesas(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteDespesa(id) {
  if (!confirm('Excluir esta despesa?')) return;
  const r = await del(`/Expenses/Delete/${id}`);
  if (r.isSuccessful) { toast('Despesa excluída!'); renderDespesas(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function markDespesaPaga(id) {
  const r = await put(`/Expenses/Update/${id}`, { paid: true });
  if (r.isSuccessful) { toast('Marcada como pago! ✅'); renderDespesas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  RECIBOS
// ═══════════════════════════════════════════════════════
async function renderRecibos() {
  setContent(loadingHtml());
  const res = await get('/Receipts/GetAll?page=1&pageSize=1000');
  const list = res.data || [];
  const total = list.reduce((s, r) => s + (r.amount || 0), 0);
  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-receipt" style="color:var(--primary);margin-right:8px"></i>Recibos</h1>
      <button class="btn btn-primary" onclick="btnNewRecibo()"><i class="fa-solid fa-plus"></i> Novo Recibo</button>
    </div>
    <div class="cards-grid" style="margin-bottom:20px">
      <div class="card"><div class="card-icon"><i class="fa-solid fa-receipt" style="color:var(--success)"></i></div><div class="card-label">Total Recebido</div><div class="card-value green">${brl(total)}</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-hashtag" style="color:var(--primary)"></i></div><div class="card-label">Quantidade</div><div class="card-value pink">${list.length}</div></div>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Produto</th><th>Data</th><th>Valor</th><th>Pagamento</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="5"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-receipt"></i></div><p>Nenhum recibo</p></div></td></tr>` :
            list.map(r => `
              <tr>
                <td><strong>${r.finalProductName||'—'}</strong></td>
                <td>${fmtDate(r.date)}</td>
                <td class="card-value green" style="font-size:.9rem">${brl(r.amount)}</td>
                <td><span class="badge badge-purple">${FORMA_PAG[r.paymentMethod]||'—'}</span></td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editRecibo(${JSON.stringify(r)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteRecibo('${r.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function btnNewRecibo()  { openModal('Novo Recibo', formRecibo(null)); }
function editRecibo(r)   { openModal('Editar Recibo', formRecibo(r)); }
function formRecibo(r) {
  const pagOpts = Object.entries(FORMA_PAG).map(([k,v]) =>
    `<option value="${k}" ${r && r.paymentMethod == k ? 'selected' : ''}>${v}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>Produto / Descrição</label><input class="form-control" id="r-prod" value="${r?.finalProductName||''}" placeholder="Ex: Bolo de Chocolate" /></div>
    <div class="form-row">
      <div class="form-group"><label>Data *</label><input class="form-control" id="r-date" type="date" value="${r?.date ? r.date.slice(0,10) : new Date().toISOString().slice(0,10)}" required /></div>
      <div class="form-group"><label>Valor (R$) *</label><input class="form-control" id="r-amount" type="number" step="0.01" value="${r?.amount||''}" placeholder="0,00" required /></div>
    </div>
    <div class="form-group"><label>Forma de Pagamento</label><select class="form-control" id="r-pag">${pagOpts}</select></div>
    <div class="form-group"><label>Descrição</label><input class="form-control" id="r-desc" value="${r?.description||''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${r ? `saveRecibo('${r.id}')` : 'createRecibo()'}">Salvar</button>
    </div>
  </div>`;
}
async function createRecibo() {
  const b = {
    finalProductName: v('r-prod')||null,
    date: v('r-date'),
    amount: parseFloat(v('r-amount'))||0,
    paymentMethod: parseInt(v('r-pag')),
    description: v('r-desc')||null,
  };
  if (!b.date || !b.amount) return toast('Data e Valor são obrigatórios', 'error');
  const r = await post('/Receipts/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Recibo criado!'); renderRecibos(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveRecibo(id) {
  const b = {
    finalProductName: v('r-prod')||null,
    date: v('r-date')||null,
    amount: parseFloat(v('r-amount'))||null,
    paymentMethod: parseInt(v('r-pag')),
    description: v('r-desc')||null,
  };
  const r = await put(`/Receipts/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Recibo atualizado!'); renderRecibos(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteRecibo(id) {
  if (!confirm('Excluir este recibo?')) return;
  const r = await del(`/Receipts/Delete/${id}`);
  if (r.isSuccessful) { toast('Recibo excluído!'); renderRecibos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  ESTOQUE (Insumos + Produtos Finais)
// ═══════════════════════════════════════════════════════
let estoqueTab = 'supplies';

async function renderEstoque() {
  setContent(loadingHtml());
  const [inv, finals] = await Promise.all([
    get('/Inventories/GetInventory'),
    get('/Inventories/GetFinalProducts?page=1&pageSize=1000'),
  ]);
  const supplies = inv.data?.supplies || [];
  const products = finals.data || [];
  const totalInv = inv.data?.totalInvested || 0;

  setContent(`
    <div class="page-header"><h1><i class="fa-solid fa-boxes-stacked" style="color:var(--primary);margin-right:8px"></i>Estoque</h1></div>
    <div class="cards-grid" style="margin-bottom:20px">
      <div class="card">
        <div class="card-icon"><i class="fa-solid fa-boxes-stacked" style="color:var(--primary)"></i></div>
        <div class="card-label">Total Investido</div>
        <div class="card-value pink">${brl(totalInv)}</div>
        <div class="card-sub">${supplies.length} insumos</div>
      </div>
      <div class="card">
        <div class="card-icon"><i class="fa-solid fa-cake-candles" style="color:var(--primary-dark)"></i></div>
        <div class="card-label">Produtos Finais</div>
        <div class="card-value purple">${products.length}</div>
        <div class="card-sub">cadastrados</div>
      </div>
    </div>
    <div class="tabs">
      <button class="tab-btn ${estoqueTab==='supplies'?'active':''}"
        onclick="switchEstoqueTab('supplies')"><i class="fa-solid fa-boxes-stacked"></i> Insumos</button>
      <button class="tab-btn ${estoqueTab==='finals'?'active':''}"
        onclick="switchEstoqueTab('finals')"><i class="fa-solid fa-cake-candles"></i> Produtos Finais</button>
    </div>
    <div id="estoque-content">
      ${estoqueTab === 'supplies' ? buildSuppliesTable(supplies) : buildFinalsTable(products)}
    </div>
  `);
  window._estoque = { supplies, products };
}

function switchEstoqueTab(tab) {
  estoqueTab = tab;
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  event.target.classList.add('active');
  const d = document.getElementById('estoque-content');
  if (tab === 'supplies') d.innerHTML = buildSuppliesTable(window._estoque?.supplies || []);
  else d.innerHTML = buildFinalsTable(window._estoque?.products || []);
}

function buildSuppliesTable(list) {
  return `
    <div class="section-header" style="margin-top:16px">
      <h2>Insumos</h2>
      <button class="btn btn-primary btn-sm" onclick="btnNewSupply()"><i class="fa-solid fa-plus"></i> Novo Insumo</button>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Nome</th><th>Qtd</th><th>Unidade</th><th>Preço Unit.</th><th>Total</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-boxes-stacked"></i></div><p>Nenhum insumo</p></div></td></tr>` :
            list.map(s => `
              <tr>
                <td><strong>${s.name||'—'}</strong></td>
                <td>${s.quantity??'—'}</td>
                <td><span class="badge badge-gray">${UNIDADE[s.unit]||s.unit}</span></td>
                <td>${brl(s.price)}</td>
                <td>${brl(s.totalPrice)}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editSupply(${JSON.stringify(s)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteSupply('${s.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `;
}

function buildFinalsTable(list) {
  return `
    <div class="section-header" style="margin-top:16px">
      <h2>Produtos Finais</h2>
      <button class="btn btn-primary btn-sm" onclick="btnNewFinal()"><i class="fa-solid fa-plus"></i> Novo Produto</button>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Nome</th><th>Descrição</th><th>Custo</th><th>Venda</th><th>Qtd Disp.</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-cake-candles"></i></div><p>Nenhum produto final</p></div></td></tr>` :
            list.map(p => `
              <tr>
                <td><strong>${p.name||'—'}</strong></td>
                <td>${trunc(p.description, 25)}</td>
                <td>${brl(p.costPrice)}</td>
                <td>${brl(p.unitPrice)}</td>
                <td>${p.quantityAvailable??'—'}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editFinal(${JSON.stringify(p)})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteFinal('${p.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `;
}

// -- Insumos --
function btnNewSupply()  { openModal('Novo Insumo', formSupply(null)); }
function editSupply(s)   { openModal('Editar Insumo', formSupply(s)); }
function formSupply(s) {
  const unitOpts = Object.entries(UNIDADE).map(([k,v]) =>
    `<option value="${k}" ${s && s.unit == k ? 'selected' : ''}>${v}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label><input class="form-control" id="s-name" value="${s?.name||''}" required /></div>
    <div class="form-row">
      <div class="form-group"><label>Quantidade</label><input class="form-control" id="s-qty" type="number" step="0.001" value="${s?.quantity||''}" /></div>
      <div class="form-group"><label>Unidade</label><select class="form-control" id="s-unit">${unitOpts}</select></div>
    </div>
    <div class="form-group"><label>Preço Unitário (R$)</label><input class="form-control" id="s-price" type="number" step="0.01" value="${s?.price||''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${s ? `saveSupply('${s.id}')` : 'createSupply()'}">Salvar</button>
    </div>
  </div>`;
}
async function createSupply() {
  const b = { name: v('s-name'), quantity: parseFloat(v('s-qty'))||null, unit: parseInt(v('s-unit')), price: parseFloat(v('s-price'))||null };
  if (!b.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Inventories/CreateSupply', b);
  if (r.isSuccessful) { closeModal(); toast('Insumo criado!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveSupply(id) {
  const b = { name: v('s-name')||null, quantity: parseFloat(v('s-qty'))||null, unit: parseInt(v('s-unit')), price: parseFloat(v('s-price'))||null };
  const r = await put(`/Inventories/UpdateSupply/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Insumo atualizado!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteSupply(id) {
  if (!confirm('Excluir este insumo?')) return;
  const r = await del(`/Inventories/DeleteSupply/${id}`);
  if (r.isSuccessful) { toast('Insumo excluído!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// -- Produtos Finais --
function btnNewFinal()  { openModal('Novo Produto Final', formFinal(null)); }
function editFinal(p)   { openModal('Editar Produto Final', formFinal(p)); }
function formFinal(p) {
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label><input class="form-control" id="f-name" value="${p?.name||''}" required /></div>
    <div class="form-group"><label>Descrição</label><input class="form-control" id="f-desc" value="${p?.description||''}" /></div>
    <div class="form-row">
      <div class="form-group"><label>Preço de Custo (R$)</label><input class="form-control" id="f-cost" type="number" step="0.01" value="${p?.costPrice||''}" /></div>
      <div class="form-group"><label>Preço de Venda (R$)</label><input class="form-control" id="f-price" type="number" step="0.01" value="${p?.unitPrice||''}" /></div>
    </div>
    <div class="form-group"><label>Quantidade Disponível</label><input class="form-control" id="f-qty" type="number" step="0.01" value="${p?.quantityAvailable||''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${p ? `saveFinal('${p.id}')` : 'createFinal()'}">Salvar</button>
    </div>
  </div>`;
}
async function createFinal() {
  const b = { name: v('f-name'), description: v('f-desc')||null, costPrice: parseFloat(v('f-cost'))||null, unitPrice: parseFloat(v('f-price'))||null, quantityAvailable: parseFloat(v('f-qty'))||null };
  if (!b.name) return toast('Nome é obrigatório', 'error');
  const r = await post('/Inventories/CreateFinalProduct', b);
  if (r.isSuccessful) { closeModal(); toast('Produto criado!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function saveFinal(id) {
  const b = { name: v('f-name')||null, description: v('f-desc')||null, costPrice: parseFloat(v('f-cost'))||null, unitPrice: parseFloat(v('f-price'))||null, quantityAvailable: parseFloat(v('f-qty'))||null };
  const r = await put(`/Inventories/UpdateFinalProduct/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Produto atualizado!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}
async function deleteFinal(id) {
  if (!confirm('Excluir este produto?')) return;
  const r = await del(`/Inventories/DeleteFinalProduct/${id}`);
  if (r.isSuccessful) { toast('Produto excluído!'); renderEstoque(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  MOVIMENTAÇÕES DE ESTOQUE
// ═══════════════════════════════════════════════════════
async function renderMovimentacoes() {
  setContent(loadingHtml());
  const res = await get('/StockMovements/GetAll?page=1&pageSize=1000');
  const list = res.data || [];
  const entradas = list.filter(m => m.type === 1).reduce((s,m)=>s+(m.quantity||0),0);
  const saidas   = list.filter(m => m.type === 0).reduce((s,m)=>s+(m.quantity||0),0);
  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-arrow-right-arrow-left" style="color:var(--primary);margin-right:8px"></i>Movimentações de Estoque</h1>
      <button class="btn btn-primary" onclick="btnNewMovimento()"><i class="fa-solid fa-plus"></i> Registrar Entrada</button>
    </div>
    <div class="cards-grid" style="margin-bottom:20px">
      <div class="card"><div class="card-icon"><i class="fa-solid fa-arrow-up" style="color:var(--success)"></i></div><div class="card-label">Entradas</div><div class="card-value green">${entradas}</div><div class="card-sub">unidades totais</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-arrow-down" style="color:var(--danger)"></i></div><div class="card-label">Saídas</div><div class="card-value red">${saidas}</div><div class="card-sub">unidades totais</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-hashtag" style="color:var(--primary)"></i></div><div class="card-label">Registros</div><div class="card-value pink">${list.length}</div></div>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Data</th><th>Tipo</th><th>Qtd</th><th>Notas</th></tr></thead>
        <tbody>
          ${list.length === 0 ? `<tr><td colspan="4"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-arrow-right-arrow-left"></i></div><p>Nenhuma movimentação</p></div></td></tr>` :
            [...list].reverse().map(m => `
              <tr>
                <td>${fmtDatetime(m.date)}</td>
                <td><span class="badge ${m.type === 1 ? 'badge-green' : 'badge-red'}">${MOV_TYPE[m.type]||'—'}</span></td>
                <td>${m.quantity}</td>
                <td>${m.notes||'—'}</td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
    <p style="font-size:.8rem;color:var(--text-muted);margin-top:12px">
      * Movimentações são imutáveis — apenas Create. Saídas são geradas automaticamente ao confirmar pedidos.
    </p>
  `);
}

function btnNewMovimento() { openModal('Registrar Movimentação', formMovimento()); }
function formMovimento() {
  const typeOpts = Object.entries(MOV_TYPE).map(([k,v]) =>
    `<option value="${k}" ${k == '1' ? 'selected' : ''}>${v}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>ID do Insumo</label><input class="form-control" id="m-supply" placeholder="GUID do insumo" /></div>
    <div class="form-row">
      <div class="form-group"><label>Quantidade *</label><input class="form-control" id="m-qty" type="number" step="0.001" required /></div>
      <div class="form-group"><label>Tipo</label><select class="form-control" id="m-type">${typeOpts}</select></div>
    </div>
    <div class="form-group"><label>Notas</label><input class="form-control" id="m-notes" placeholder="Ex: Compra de farinha de trigo" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="createMovimento()">Salvar</button>
    </div>
  </div>`;
}
async function createMovimento() {
  const b = {
    supplyId: v('m-supply')||null,
    quantity: parseFloat(v('m-qty'))||0,
    type: parseInt(v('m-type')),
    notes: v('m-notes')||null,
  };
  if (!b.quantity) return toast('Quantidade é obrigatória', 'error');
  const r = await post('/StockMovements/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Movimentação registrada!'); renderMovimentacoes(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ── Utilidade ─────────────────────────────────────────
function v(id) { const el = document.getElementById(id); return el ? el.value : ''; }
