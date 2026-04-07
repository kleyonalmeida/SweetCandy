/* =====================================================
   SweetCandy — Frontend SPA
   ===================================================== */

const API = '/api';

let currentPage = 'dashboard';
const state = { page: 1, pageSize: 20 };

const now = new Date();
let dashMonth = now.getMonth();
let dashYear  = now.getFullYear();
let valuesHidden = false;

const MESES = ['Janeiro','Fevereiro','Março','Abril','Maio','Junho',
               'Julho','Agosto','Setembro','Outubro','Novembro','Dezembro'];

const STATUS_ORDER = { 0: 'Pendente', 1: 'Confirmada', 2: 'Cancelada', 3: 'Concluída' };
const STATUS_CLASS  = { 0: 's-0', 1: 's-1', 2: 's-2', 3: 's-3' };
const FORMA_PAG    = { 0: 'Dinheiro', 1: 'Débito', 2: 'Crédito', 3: 'Pix' };
const FORMA_PAG_ICON = { 0: 'fa-money-bill-wave', 1: 'fa-credit-card', 2: 'fa-credit-card', 3: 'fa-qrcode' };
const UNIDADE      = { 0: 'Un', 1: 'Kg', 2: 'G', 3: 'L', 4: 'Ml', 5: 'Mg', 6: 'Cx', 7: 'Pct' };
const MOV_TYPE     = { 0: 'Saída', 1: 'Entrada' };

// ── Inicialização ─────────────────────────────────────
window.addEventListener('DOMContentLoaded', () => {
  document.getElementById('topbar-date').textContent =
    new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' });

  document.querySelectorAll('.nav-item').forEach(el => {
    el.addEventListener('click', () => navigate(el.dataset.page));
  });

  navigate('dashboard');
});

// ── Navegação ─────────────────────────────────────────
function navigate(page) {
  currentPage = page;
  document.querySelectorAll('.nav-item').forEach(el =>
    el.classList.toggle('active', el.dataset.page === page));
  const titles = {
    dashboard: 'Painel',
    financas: 'Finanças',
    'gestao-caixa': 'Gestão de Caixa',
    pedidos: 'Pedidos',
    estoque: 'Estoque',
    orcamentos: 'Orçamentos',
    clientes: 'Clientes',
  };
  document.getElementById('topbar-title').textContent = titles[page] || page;
  const renders = {
    dashboard: renderDashboard,
    financas: renderFinancas,
    'gestao-caixa': renderGestaoCaixa,
    pedidos: renderPedidos,
    estoque: renderEstoque,
    orcamentos: renderOrcamentos,
    clientes: () => renderCrud('clientes'),
  };
  (renders[page] || (() => {}))();
}

function toggleSidebar() {
  const sb  = document.getElementById('sidebar');
  const sw  = document.getElementById('sidebar-wrapper');
  const ov  = document.getElementById('sidebar-overlay');
  if (window.innerWidth <= 768) {
    sw.classList.toggle('open');
    ov.classList.toggle('open');
  } else {
    sb.classList.toggle('collapsed');
  }
}

// ── API ────────────────────────────────────────────────
async function api(method, path, body) {
  const opts = { method, headers: { 'Content-Type': 'application/json' } };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(API + path, opts);
  return res.json().catch(() => ({ isSuccessful: false, messages: ['Erro de resposta'] }));
}

const get  = (path)       => api('GET', path);
const post = (path, body) => api('POST', path, body);
const put  = (path, body) => api('PUT', path, body);
const del  = (path)       => api('DELETE', path);

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

// ── Helpers ───────────────────────────────────────────
function setContent(html) { document.getElementById('content').innerHTML = html; }
function loadingHtml() { return `<div class="loading"><div class="spinner"></div><br>Carregando...</div>`; }

const brl = v => v != null
  ? `R$ ${Number(v).toFixed(2).replace('.', ',').replace(/\B(?=(\d{3})+(?!\d))/g, '.')}`
  : '—';
const brlH    = v => valuesHidden ? 'R$ ••••' : brl(v);
const fmtDate = s => s ? new Date(s).toLocaleDateString('pt-BR') : '—';
const fmtDateShort = s => {
  if (!s) return '—';
  const d = new Date(s);
  return `${String(d.getDate()).padStart(2,'0')}/${String(d.getMonth()+1).padStart(2,'0')}`;
};
const fmtDatetime = s => s ? new Date(s).toLocaleString('pt-BR') : '—';
const trunc = (s, n = 30) => s && s.length > n ? s.slice(0, n) + '…' : (s || '—');
const v = id => { const el = document.getElementById(id); return el ? el.value : ''; };

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

  const [dash, orders, customers, receipts] = await Promise.all([
    get(`/Dashboard?year=${dashYear}&month=${dashMonth + 1}`),
    get('/Orders/GetAll?page=1&pageSize=1000'),
    get('/Customers/GetAll?page=1&pageSize=1000'),
    get('/Receipts/GetAll?page=1&pageSize=1000'),
  ]);

  const d = dash.data || {};
  const totalReceitas      = d.revenue    ?? 0;
  const totalDespesas      = d.expenses   ?? 0;
  const lucro              = d.profit     ?? 0;
  const effectiveGoal      = d.effectiveGoal      ?? 0;
  const effectiveGoalPct   = d.effectiveGoalPercent ?? 0;
  const suggestedGoal      = d.suggestedGoal ?? 0;
  const isCustomGoal       = d.monthlyGoalTarget != null;
  const metaBarPct         = Math.min(100, Math.round(effectiveGoalPct));

  const pedidosPendentes   = (orders.data || []).filter(o => o.status === 0).length;
  const pedidosConfirmados = (orders.data || []).filter(o => o.status === 1).length;
  const totalClientes      = (customers.data || []).length;
  const allReceipts        = receipts.data || [];

  const eyeIcon = valuesHidden
    ? '<i class="fa-solid fa-eye-slash"></i> Exibir'
    : '<i class="fa-solid fa-eye"></i> Ocultar';

  const metaLabel = isCustomGoal ? 'meta personalizada' : 'sugestão automática';

  // ── Dados semanais do mês para o gráfico ──────────────
  const semanas = calcWeeklyProfit(allReceipts, dashMonth, dashYear);

  setContent(`
    <section class="welcome-section">
      <p class="greeting">Bem-vinda de volta 👋</p>
      <h1>Painel SweetCandy</h1>
      <div class="month-nav">
        <button class="month-nav-btn" onclick="changeMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
        <span class="month-nav-label">${MESES[dashMonth]} de ${dashYear}</span>
        <button class="month-nav-btn" onclick="changeMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
        <button class="hide-values-btn" onclick="toggleValues()">${eyeIcon}</button>
      </div>
    </section>

    <div class="dash-layout">

      <!-- COLUNA PRINCIPAL -->
      <div class="dash-main">

        <!-- Stat chips pequenos -->
        <div class="stat-chips">
          <div class="stat-chip" onclick="navigate('financas')" title="Ver finanças">
            <div class="stat-chip-label">Receitas</div>
            <div class="stat-chip-value green">${brlH(totalReceitas)}</div>
            <div class="stat-chip-delta"><i class="fa-solid fa-arrow-trend-up" style="color:var(--success)"></i> ${MESES[dashMonth]}</div>
          </div>
          <div class="stat-chip" onclick="navigate('financas')" title="Ver finanças">
            <div class="stat-chip-label">Despesas</div>
            <div class="stat-chip-value red">${brlH(totalDespesas)}</div>
            <div class="stat-chip-delta"><i class="fa-solid fa-arrow-trend-down" style="color:var(--danger)"></i> ${MESES[dashMonth]}</div>
          </div>
          <div class="stat-chip">
            <div class="stat-chip-label">Lucro</div>
            <div class="stat-chip-value ${lucro >= 0 ? 'orange' : 'red'}">${brlH(lucro)}</div>
            <div class="stat-chip-delta"><i class="fa-solid fa-scale-balanced" style="color:var(--primary)"></i> ${MESES[dashMonth]}</div>
          </div>
        </div>

        <!-- Gráfico de lucro semanal -->
        <div class="chart-card">
          <div class="chart-header">
            <span class="chart-title">Receitas por semana</span>
            <div class="chart-legend">
              <span><span class="chart-legend-dot" style="background:var(--primary)"></span>Este mês</span>
            </div>
          </div>
          <div class="chart-canvas-wrap">
            <canvas id="dash-chart"></canvas>
          </div>
        </div>

        <!-- Meta mensal -->
        <div class="meta-card">
          <div class="meta-card-header">
            <span class="meta-card-title"><i class="fa-solid fa-bullseye"></i> Meta Mensal</span>
            <div style="display:flex;align-items:center;gap:.5rem">
              <span class="meta-pct">${metaBarPct}%</span>
              <button class="btn btn-secondary" style="padding:.25rem .6rem;font-size:.75rem" onclick="btnConfigMeta()">
                <i class="fa-solid fa-gear"></i> Configurar
              </button>
            </div>
          </div>
          <div class="progress-track">
            <div class="progress-fill" style="width:${metaBarPct}%"></div>
          </div>
          <div class="meta-details">
            <div class="meta-detail">
              <span class="meta-detail-label">Realizado</span>
              <span class="meta-detail-value">${brlH(totalReceitas)}</span>
            </div>
            <div class="meta-detail">
              <span class="meta-detail-label">Meta</span>
              <span class="meta-detail-value" title="${metaLabel}">${brlH(effectiveGoal)}</span>
            </div>
            <div class="meta-detail">
              <span class="meta-detail-label">Falta</span>
              <span class="meta-detail-value">${brlH(Math.max(0, effectiveGoal - totalReceitas))}</span>
            </div>
          </div>
          ${!isCustomGoal && suggestedGoal > 0 ? `<p style="font-size:.75rem;color:var(--text-muted);margin-top:.5rem">
            <i class="fa-solid fa-circle-info"></i> Meta calculada automaticamente: gastos × 1,5.
            <a href="#" onclick="btnConfigMeta();return false">Personalizar</a>
          </p>` : ''}
        </div>

        <!-- Pedidos recentes -->
        ${(orders.data||[]).length > 0 ? `
        <div>
          <div class="section-header">
            <h2>Pedidos recentes</h2>
            <a class="btn btn-sm btn-secondary" onclick="navigate('pedidos')">Ver todos →</a>
          </div>
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
        </div>` : ''}

      </div><!-- /dash-main -->

      <!-- COLUNA LATERAL -->
      <div class="dash-side">

        <!-- Resumo operacional -->
        <div class="side-card">
          <div class="side-card-title">Operacional</div>
          <div class="side-stat-row">
            <span class="side-stat-name">
              <i class="fa-solid fa-clipboard-list"></i>
              Pedidos pendentes
            </span>
            <span class="side-stat-val">${pedidosPendentes}</span>
          </div>
          <div class="side-stat-row">
            <span class="side-stat-name">
              <i class="fa-solid fa-circle-check" style="background:#dcfce7;color:#16a34a"></i>
              Confirmados
            </span>
            <span class="side-stat-val" style="color:var(--success)">${pedidosConfirmados}</span>
          </div>
          <div class="side-stat-row">
            <span class="side-stat-name">
              <i class="fa-solid fa-users"></i>
              Clientes
            </span>
            <span class="side-stat-val">${totalClientes}</span>
          </div>
        </div>

        <!-- Meta resumida -->
        <div class="side-card" onclick="btnConfigMeta()" style="cursor:pointer" title="Configurar meta">
          <div class="side-card-title">Meta do mês</div>
          <div class="side-meta-pct">${metaBarPct}%</div>
          <div class="side-meta-label">${brlH(totalReceitas)} de ${brlH(effectiveGoal)}</div>
          <div class="progress-track" style="margin-bottom:0">
            <div class="progress-fill" style="width:${metaBarPct}%"></div>
          </div>
        </div>

        <!-- Ações rápidas compactas -->
        <div class="side-card">
          <div class="side-card-title">Ações rápidas</div>
          <div class="side-actions">
            <button class="side-action-btn" onclick="navigate('financas'); setTimeout(()=>btnNewTransacao('receita'),300)">
              <i class="fa-solid fa-arrow-trend-up"></i> Nova receita
            </button>
            <button class="side-action-btn" onclick="navigate('financas'); setTimeout(()=>btnNewTransacao('despesa'),300)">
              <i class="fa-solid fa-arrow-trend-down" style="background:#fee2e2;color:var(--danger)"></i> Nova despesa
            </button>
            <button class="side-action-btn" onclick="navigate('pedidos'); setTimeout(()=>btnNewPedido(),300)">
              <i class="fa-solid fa-clipboard-list"></i> Novo pedido
            </button>
            <button class="side-action-btn" onclick="navigate('orcamentos'); setTimeout(()=>btnNewOrcamento(),300)">
              <i class="fa-solid fa-file-invoice"></i> Novo orçamento
            </button>
            <button class="side-action-btn" onclick="navigate('clientes'); setTimeout(()=>btnNewCliente(),300)">
              <i class="fa-solid fa-user-plus"></i> Novo cliente
            </button>
            <button class="side-action-btn" onclick="navigate('estoque')">
              <i class="fa-solid fa-boxes-stacked"></i> Ver estoque
            </button>
          </div>
        </div>

      </div><!-- /dash-side -->

    </div><!-- /dash-layout -->
  `);

  // Renderizar gráfico após DOM estar pronto
  requestAnimationFrame(() => renderDashChart(semanas));
}

// Calcula receita total por semana do mês para o gráfico
function calcWeeklyProfit(allReceipts, month, year) {
  // Determina número de semanas do mês (agrupando por semana ISO dentro do mês)
  const firstDay = new Date(year, month, 1);
  const lastDay  = new Date(year, month + 1, 0);
  const weeks = [];
  let cur = new Date(firstDay);
  let weekNum = 1;
  while (cur <= lastDay) {
    const start = new Date(cur);
    const end   = new Date(cur);
    end.setDate(end.getDate() + 6);
    if (end > lastDay) end.setTime(lastDay.getTime());
    weeks.push({ label: `Sem ${weekNum}`, start, end, total: 0 });
    cur.setDate(cur.getDate() + 7);
    weekNum++;
  }
  for (const r of allReceipts) {
    const d = new Date(r.date || r.receiptDate || r.createdAt || '');
    if (isNaN(d)) continue;
    if (d.getMonth() !== month || d.getFullYear() !== year) continue;
    for (const w of weeks) {
      if (d >= w.start && d <= w.end) { w.total += r.amount ?? r.value ?? 0; break; }
    }
  }
  return weeks;
}

// Desenha o gráfico de linha com Chart.js
let _dashChart = null;
function renderDashChart(weeks) {
  const canvas = document.getElementById('dash-chart');
  if (!canvas) return;
  if (_dashChart) { _dashChart.destroy(); _dashChart = null; }
  _dashChart = new Chart(canvas, {
    type: 'line',
    data: {
      labels: weeks.map(w => w.label),
      datasets: [{
        label: 'Receitas',
        data: weeks.map(w => w.total),
        borderColor: '#e8663c',
        backgroundColor: 'rgba(232,102,60,.08)',
        tension: 0.4,
        fill: true,
        pointRadius: 4,
        pointBackgroundColor: '#e8663c',
        pointBorderColor: '#fff',
        pointBorderWidth: 2,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false }, tooltip: { mode: 'index', intersect: false } },
      scales: {
        x: { grid: { display: false }, ticks: { color: '#9ca3af', font: { size: 11 } } },
        y: { grid: { color: 'rgba(0,0,0,.04)' }, ticks: { color: '#9ca3af', font: { size: 11 }, callback: v => 'R$' + v.toLocaleString('pt-BR') } }
      }
    }
  });
}

function btnConfigMeta() {
  openModal('Configurar Meta Mensal', `
    <div class="form-grid">
      <div class="form-group">
        <label>Modo</label>
        <select class="form-control" id="meta-modo" onchange="toggleMetaModo()">
          <option value="valor">Valor fixo (R$)</option>
          <option value="pct">Porcentagem sobre os gastos (%)</option>
        </select>
      </div>
      <div class="form-group" id="meta-valor-group">
        <label>Valor da meta (R$)</label>
        <input class="form-control" id="meta-valor" type="number" step="0.01" min="0" placeholder="Ex: 3000.00" />
      </div>
      <div class="form-group" id="meta-pct-group" style="display:none">
        <label>Porcentagem sobre os gastos (%)</label>
        <input class="form-control" id="meta-pct-input" type="number" step="1" min="0" max="1000" placeholder="Ex: 50 = gastos × 1,5" />
        <small style="color:var(--text-muted)">50% significa: meta = gastos do mês + 50%</small>
      </div>
      <div class="modal-footer">
        <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
        <button class="btn btn-primary" onclick="saveGoal()">Salvar Meta</button>
      </div>
    </div>
  `);
}

function toggleMetaModo() {
  const modo = document.getElementById('meta-modo')?.value;
  document.getElementById('meta-valor-group').style.display = modo === 'valor' ? '' : 'none';
  document.getElementById('meta-pct-group').style.display   = modo === 'pct'   ? '' : 'none';
}

async function saveGoal() {
  const modo = document.getElementById('meta-modo')?.value;
  const body = { year: dashYear, month: dashMonth + 1 };

  if (modo === 'pct') {
    const pct = parseFloat(document.getElementById('meta-pct-input')?.value);
    body.percentageOverCosts = isNaN(pct) ? null : pct;
  } else {
    const val = parseFloat(document.getElementById('meta-valor')?.value);
    body.targetAmount = isNaN(val) ? null : val;
  }

  const r = await post('/MonthlyGoals', body);
  if (r.isSuccessful) { closeModal(); toast('Meta salva!'); renderDashboard(); }
  else toast((r.messages || []).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  FINANÇAS
// ═══════════════════════════════════════════════════════
let _finReceitas = [];
let _finDespesas = [];

async function renderFinancas() {
  setContent(loadingHtml());
  const [receipts, expenses] = await Promise.all([
    get('/Receipts/GetAll?page=1&pageSize=1000'),
    get('/Expenses/GetAll?page=1&pageSize=1000'),
  ]);
  _finReceitas = receipts.data || [];
  _finDespesas = expenses.data || [];
  const totalR = _finReceitas.reduce((s, r) => s + (r.amount || 0), 0);
  const totalE = _finDespesas.reduce((s, e) => s + (e.value || 0), 0);

  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-wallet" style="color:var(--primary);margin-right:8px"></i>Finanças</h1>
      <button class="btn btn-primary" onclick="btnNewTransacao('receita')">
        <i class="fa-solid fa-plus"></i> Adicionar
      </button>
    </div>

    <div class="summary-cards" style="margin-bottom:24px">
      <div class="summary-card">
        <div class="sc-icon green"><i class="fa-solid fa-arrow-trend-up"></i></div>
        <div class="sc-body">
          <div class="sc-label">Receitas <span style="background:var(--primary);color:#fff;border-radius:999px;padding:1px 7px;font-size:.7rem;font-weight:700;margin-left:6px">${_finReceitas.length}</span></div>
          <div class="sc-value green">${brl(totalR)}</div>
        </div>
      </div>
      <div class="summary-card">
        <div class="sc-icon red"><i class="fa-solid fa-arrow-trend-down"></i></div>
        <div class="sc-body">
          <div class="sc-label">Despesas <span style="background:var(--danger);color:#fff;border-radius:999px;padding:1px 7px;font-size:.7rem;font-weight:700;margin-left:6px">${_finDespesas.length}</span></div>
          <div class="sc-value red">${brl(totalE)}</div>
        </div>
      </div>
      <div class="summary-card">
        <div class="sc-icon orange"><i class="fa-solid fa-scale-balanced"></i></div>
        <div class="sc-body">
          <div class="sc-label">Lucro</div>
          <div class="sc-value ${totalR - totalE >= 0 ? 'orange' : 'red'}">${brl(totalR - totalE)}</div>
        </div>
      </div>
    </div>

    <div class="tabs">
      <button class="tab-btn active" id="tab-todas"    onclick="finTab('todas')">Todas</button>
      <button class="tab-btn"        id="tab-receitas" onclick="finTab('receitas')">Receitas</button>
      <button class="tab-btn"        id="tab-despesas" onclick="finTab('despesas')">Despesas</button>
    </div>

    <div id="fin-list">
      ${buildFinList(_finReceitas, _finDespesas, 'todas')}
    </div>
  `);
}

function finTab(tab) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('tab-' + tab).classList.add('active');
  document.getElementById('fin-list').innerHTML = buildFinList(_finReceitas, _finDespesas, tab);
}

function buildFinList(receipts, expenses, tab) {
  let items = [];

  if (tab !== 'despesas') {
    receipts.forEach(r => items.push({ tipo: 'receita', date: r.date, obj: r }));
  }
  if (tab !== 'receitas') {
    expenses.forEach(e => items.push({ tipo: 'despesa', date: e.date, obj: e }));
  }

  if (items.length === 0) {
    return `<div class="empty-state">
      <div class="empty-icon"><i class="fa-solid fa-wallet"></i></div>
      <p>Nenhuma transação registrada ainda.</p>
    </div>`;
  }

  items.sort((a, b) => new Date(b.date) - new Date(a.date));

  return items.map(item => {
    if (item.tipo === 'receita') {
      const r = item.obj;
      return `<div class="tx-item">
        <div class="tx-icon receita"><i class="fa-solid fa-arrow-trend-up"></i></div>
        <div class="tx-info">
          <strong>${r.finalProductName || 'Recibo'}</strong>
          <span>${fmtDate(r.date)} · ${FORMA_PAG[r.paymentMethod] || '—'}</span>
        </div>
        <div class="tx-value receita">+ ${brl(r.amount)}</div>
        <div style="display:flex;gap:4px;margin-left:8px">
          <button class="btn btn-sm btn-secondary btn-icon" onclick='editRecibo(${JSON.stringify(r).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
          <button class="btn btn-sm btn-danger btn-icon" onclick="deleteRecibo('${r.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
        </div>
      </div>`;
    } else {
      const e = item.obj;
      return `<div class="tx-item">
        <div class="tx-icon despesa"><i class="fa-solid fa-arrow-trend-down"></i></div>
        <div class="tx-info">
          <strong>${e.name || '—'}</strong>
          <span>${fmtDate(e.date)} · ${FORMA_PAG[e.paymentMethod] || '—'}${e.categoryName ? ` · <i class="fa-solid fa-tag" style="font-size:.7rem"></i> ${e.categoryName}` : ''} · ${e.paid
            ? '<i class="fa-solid fa-check" style="color:var(--success)"></i> Pago'
            : '<i class="fa-regular fa-clock" style="color:var(--warning)"></i> Pendente'}</span>
        </div>
        <div class="tx-value despesa">− ${brl(e.value)}</div>
        <div style="display:flex;gap:4px;margin-left:8px">
          ${!e.paid ? `<button class="btn btn-sm btn-success btn-icon" onclick="markDespesaPaga('${e.id}')" title="Marcar pago"><i class="fa-solid fa-check"></i></button>` : ''}
          <button class="btn btn-sm btn-secondary btn-icon" onclick='editDespesa(${JSON.stringify(e).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
          <button class="btn btn-sm btn-danger btn-icon" onclick="deleteDespesa('${e.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
        </div>
      </div>`;
    }
  }).join('');
}

// ── Modal Nova Transação ──────────────────────────────
let _txTipo = 'receita';

function btnNewTransacao(tipo = 'receita') {
  _txTipo = tipo;
  openModal('Nova Transação', buildTransacaoForm());
}

function setTxTipo(tipo) {
  _txTipo = tipo;
  document.getElementById('tx-toggle-receita').className =
    tipo === 'receita' ? 'active-receita' : '';
  document.getElementById('tx-toggle-despesa').className =
    tipo === 'despesa' ? 'active-despesa' : '';
  document.getElementById('tx-fields-receita').style.display =
    tipo === 'receita' ? '' : 'none';
  document.getElementById('tx-fields-despesa').style.display =
    tipo === 'despesa' ? '' : 'none';
}

function buildTransacaoForm() {
  const pagOpts = Object.entries(FORMA_PAG).map(([k, val]) =>
    `<option value="${k}">${val}</option>`).join('');
  return `
    <div class="transaction-toggle">
      <button id="tx-toggle-receita" class="${_txTipo === 'receita' ? 'active-receita' : ''}"
        onclick="setTxTipo('receita')">
        <i class="fa-solid fa-arrow-trend-up"></i> Receita
      </button>
      <button id="tx-toggle-despesa" class="${_txTipo === 'despesa' ? 'active-despesa' : ''}"
        onclick="setTxTipo('despesa')">
        <i class="fa-solid fa-arrow-trend-down"></i> Despesa
      </button>
    </div>

    <div class="form-grid">
      <div class="form-group">
        <label>Valor (R$) *</label>
        <input class="form-control" id="tx-valor" type="number" step="0.01" placeholder="0,00" required />
      </div>

      <div class="form-group">
        <label>Data</label>
        <input class="form-control" id="tx-data" type="date"
          value="${new Date().toISOString().slice(0,10)}" />
      </div>

      <div class="form-group">
        <label>Método de pagamento</label>
        <select class="form-control" id="tx-metodo">${pagOpts}</select>
      </div>

      <!-- Campos específicos Receita -->
      <div id="tx-fields-receita" style="display:${_txTipo === 'receita' ? '' : 'none'}">
        <div class="form-group" style="margin-bottom:14px">
          <label>Produto / Descrição (Receita)</label>
          <input class="form-control" id="tx-r-prod" placeholder="Ex: Bolo de Chocolate" />
        </div>
        <div class="form-group">
          <label>Descrição adicional</label>
          <input class="form-control" id="tx-r-desc" placeholder="Detalhes..." />
        </div>
      </div>

      <!-- Campos específicos Despesa -->
      <div id="tx-fields-despesa" style="display:${_txTipo === 'despesa' ? '' : 'none'}">
        <div class="form-group" style="margin-bottom:14px">
          <label>Nome da Despesa *</label>
          <input class="form-control" id="tx-e-nome" placeholder="Ex: Conta de Luz" />
        </div>
        <div class="form-group">
          <label>Categoria</label>
          <input class="form-control" id="tx-e-cat" placeholder="Ex: Insumos, Aluguel..." />
        </div>
      </div>

      <div class="modal-footer">
        <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
        <button class="btn btn-primary" onclick="saveTransacao()">Salvar</button>
      </div>
    </div>
  `;
}

async function saveTransacao() {
  const valor   = parseFloat(v('tx-valor'));
  const data    = v('tx-data');
  const metodo  = parseInt(v('tx-metodo'));

  if (_txTipo === 'receita') {
    const body = {
      finalProductName: v('tx-r-prod') || null,
      date: data,
      amount: valor,
      paymentMethod: metodo,
      description: v('tx-r-desc') || null,
    };
    const r = await post('/Receipts/Create', body);
    if (r.isSuccessful) { closeModal(); toast('Receita adicionada!'); renderFinancas(); }
    else toast((r.messages||[]).join(', '), 'error');
  } else {
    const body = {
      name: v('tx-e-nome'),
      value: valor,
      paid: false,
      date: data || new Date().toISOString(),
      categoryName: v('tx-e-cat') || null,
      paymentMethod: metodo,
    };
    const r = await post('/Expenses/Create', body);
    if (r.isSuccessful) { closeModal(); toast('Despesa adicionada!'); renderFinancas(); }
    else toast((r.messages||[]).join(', '), 'error');
  }
}

// Recibos (edit/delete mantidos para uso nas ações da lista)
function editRecibo(r)   { openModal('Editar Receita', formRecibo(r)); }
async function editDespesa(e) {
  const cats = await get('/Categories/GetAll');
  const catOpts = '<option value="">— sem categoria —</option>' +
    (cats.data || []).map(c =>
      `<option value="${c.id}" ${e?.categoryId === c.id ? 'selected' : ''}>${c.name}</option>`
    ).join('');
  openModal('Editar Despesa', formDespesa(e, catOpts));
}

function formRecibo(r) {
  const pagOpts = Object.entries(FORMA_PAG).map(([k, val]) =>
    `<option value="${k}" ${r && r.paymentMethod == k ? 'selected' : ''}>${val}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>Produto / Descrição</label>
      <input class="form-control" id="r-prod" value="${r?.finalProductName||''}" /></div>
    <div class="form-row">
      <div class="form-group"><label>Data *</label>
        <input class="form-control" id="r-date" type="date"
          value="${r?.date ? r.date.slice(0,10) : new Date().toISOString().slice(0,10)}" required /></div>
      <div class="form-group"><label>Valor (R$) *</label>
        <input class="form-control" id="r-amount" type="number" step="0.01"
          value="${r?.amount||''}" placeholder="0,00" required /></div>
    </div>
    <div class="form-group"><label>Forma de Pagamento</label>
      <select class="form-control" id="r-pag">${pagOpts}</select></div>
    <div class="form-group"><label>Descrição</label>
      <input class="form-control" id="r-desc" value="${r?.description||''}" /></div>
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
  const r = await post('/Receipts/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Recibo criado!'); renderFinancas(); }
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
  if (r.isSuccessful) { closeModal(); toast('Receita atualizada!'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function deleteRecibo(id) {
  if (!confirm('Excluir esta receita?')) return;
  const r = await del(`/Receipts/Delete/${id}`);
  if (r.isSuccessful) { toast('Receita excluída!'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

function formDespesa(e, catOpts = '') {
  const pagOpts = Object.entries(FORMA_PAG).map(([k, val]) =>
    `<option value="${k}" ${e && e.paymentMethod == k ? 'selected' : ''}>${val}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label>
      <input class="form-control" id="e-name" value="${e?.name||''}" required /></div>
    <div class="form-group"><label>Valor (R$) *</label>
      <input class="form-control" id="e-val" type="number" step="0.01"
        value="${e?.value||''}" placeholder="0,00" required /></div>
    ${catOpts ? `<div class="form-group"><label>Categoria</label>
      <select class="form-control" id="e-cat">${catOpts}</select></div>` : ''}
    <div class="form-group"><label>Método de Pagamento</label>
      <select class="form-control" id="e-pag">${pagOpts}</select></div>
    <div class="form-group"><label>Status</label>
      <select class="form-control" id="e-paid">
        <option value="false" ${!e?.paid ? 'selected' : ''}>Pendente</option>
        <option value="true"  ${e?.paid ? 'selected' : ''}>Pago</option>
      </select></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${e ? `saveDespesa('${e.id}')` : 'createDespesa()'}">Salvar</button>
    </div>
  </div>`;
}

async function createDespesa() {
  const b = {
    name: v('e-name'),
    value: parseFloat(v('e-val'))||0,
    paid: v('e-paid') === 'true',
    categoryId: v('e-cat') || null,
    paymentMethod: parseInt(v('e-pag')||'0'),
  };
  const r = await post('/Expenses/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Despesa criada!'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function saveDespesa(id) {
  const b = {
    name: v('e-name')||null,
    value: parseFloat(v('e-val'))||null,
    paid: v('e-paid') === 'true',
    categoryId: v('e-cat') || null,
    paymentMethod: parseInt(v('e-pag')||'0'),
  };
  const r = await put(`/Expenses/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Despesa atualizada!'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function deleteDespesa(id) {
  if (!confirm('Excluir esta despesa?')) return;
  const r = await del(`/Expenses/Delete/${id}`);
  if (r.isSuccessful) { toast('Despesa excluída!'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function markDespesaPaga(id) {
  const r = await put(`/Expenses/Update/${id}`, { paid: true });
  if (r.isSuccessful) { toast('Marcada como pago! ✅'); renderFinancas(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// ═══════════════════════════════════════════════════════
//  GESTÃO DE CAIXA
// ═══════════════════════════════════════════════════════
let _caixaTab = 'entradas';

async function renderGestaoCaixa() {
  setContent(loadingHtml());

  const [receipts, expenses] = await Promise.all([
    get('/Receipts/GetAll?page=1&pageSize=1000'),
    get('/Expenses/GetAll?page=1&pageSize=1000'),
  ]);

  const rList = receipts.data || [];
  const eList = expenses.data || [];

  const totalEntradas = rList.reduce((s, r) => s + (r.amount || 0), 0);
  const totalSaidas   = eList.reduce((s, e) => s + (e.value || 0), 0);
  const saldoTotal    = totalEntradas - totalSaidas;

  // Agrupa por método de pagamento
  const groups = {};
  rList.forEach(r => {
    const key = r.paymentMethod ?? 0;
    if (!groups[key]) groups[key] = { entradas: [], saidas: [] };
    groups[key].entradas.push(r);
  });
  eList.forEach(e => {
    const key = e.paymentMethod ?? 0;
    if (!groups[key]) groups[key] = { entradas: [], saidas: [] };
    groups[key].saidas.push(e);
  });

  // Resumo para a tab ativa
  const buildResumoPorMetodo = (tab) => {
    return Object.entries(groups).map(([key, g]) => {
      const items = tab === 'entradas' ? g.entradas : g.saidas;
      const total = tab === 'entradas'
        ? items.reduce((s, r) => s + (r.amount || 0), 0)
        : items.reduce((s, e) => s + (e.value || 0), 0);
      if (total === 0) return '';
      return `<div class="method-summary-item">
        <div class="method-summary-icon"><i class="fa-solid ${FORMA_PAG_ICON[key] || 'fa-wallet'}"></i></div>
        <span class="method-summary-name">${FORMA_PAG[key] || '—'}</span>
        <span class="method-summary-value ${tab === 'saidas' ? 'out' : ''}">${brl(total)}</span>
        <i class="fa-solid fa-chevron-right" style="color:var(--text-muted);font-size:.75rem"></i>
      </div>`;
    }).join('');
  };

  // Cards por método
  const methodCards = Object.entries(groups).map(([key, g]) => {
    const totalIn  = g.entradas.reduce((s, r) => s + (r.amount || 0), 0);
    const totalOut = g.saidas.reduce((s, e) => s + (e.value || 0), 0);
    const saldo    = totalIn - totalOut;
    const count    = g.entradas.length + g.saidas.length;

    // Últimas 3 transações mescladas
    const allTx = [
      ...g.entradas.map(r => ({ tipo: 'in', nome: r.finalProductName || 'Receita', valor: r.amount, data: r.date })),
      ...g.saidas.map(e => ({ tipo: 'out', nome: e.name || 'Despesa', valor: e.value, data: e.date })),
    ].sort((a, b) => new Date(b.data) - new Date(a.data)).slice(0, 3);

    const recentItens = allTx.map(t => `
      <div class="caixa-recent-item">
        <div class="caixa-recent-sign ${t.tipo}">${t.tipo === 'in' ? '+' : '-'}</div>
        <span class="caixa-recent-name">${trunc(t.nome, 18).toUpperCase()}</span>
        <span class="caixa-recent-amount ${t.tipo}">${t.tipo === 'in' ? '+' : '-'}${brl(t.valor)}</span>
        <span class="caixa-recent-date">${fmtDateShort(t.data)}</span>
      </div>`).join('');

    return `
      <div class="caixa-method-card">
        <div class="caixa-method-header">
          <div class="caixa-method-identity">
            <div class="caixa-method-icon ${saldo < 0 ? 'negative' : ''}">
              <i class="fa-solid ${FORMA_PAG_ICON[key] || 'fa-wallet'}"></i>
            </div>
            <div>
              <div class="caixa-method-name">${FORMA_PAG[key] || '—'}</div>
              <div class="caixa-method-count">${count} transaç${count === 1 ? 'ão' : 'ões'}</div>
            </div>
          </div>
          <div class="caixa-method-saldo-wrap">
            <div class="caixa-method-saldo ${saldo >= 0 ? 'positive' : 'negative'}">${brl(saldo)}</div>
            <div class="caixa-method-saldo-label">Saldo atual</div>
          </div>
        </div>

        <div class="caixa-method-stats">
          <div class="caixa-stat">
            <span class="caixa-stat-label"><i class="fa-solid fa-arrow-up-right"></i> Entradas</span>
            <span class="caixa-stat-value in">${brl(totalIn)}</span>
          </div>
          <div class="caixa-stat">
            <span class="caixa-stat-label"><i class="fa-solid fa-arrow-down-right"></i> Saídas</span>
            <span class="caixa-stat-value out">${brl(totalOut)}</span>
          </div>
        </div>

        ${allTx.length > 0 ? `
          <div class="caixa-recent-title">Transações recentes</div>
          ${recentItens}
        ` : ''}
      </div>`;
  }).join('');

  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-cash-register" style="color:var(--primary);margin-right:8px"></i>Gestão de Caixa</h1>
      <button class="btn btn-primary" onclick="btnNewTransacao('receita')">
        <i class="fa-solid fa-plus"></i> Nova Transação
      </button>
    </div>

    <div class="summary-cards" style="margin-bottom:24px">
      <div class="summary-card">
        <div class="sc-icon orange"><i class="fa-solid fa-wallet"></i></div>
        <div class="sc-body">
          <div class="sc-label">Saldo Total</div>
          <div class="sc-value ${saldoTotal >= 0 ? 'orange' : 'red'}">${brl(saldoTotal)}</div>
        </div>
      </div>
      <div class="summary-card">
        <div class="sc-icon green"><i class="fa-solid fa-arrow-trend-up"></i></div>
        <div class="sc-body">
          <div class="sc-label">Total de Entradas</div>
          <div class="sc-value green">${brl(totalEntradas)}</div>
        </div>
      </div>
      <div class="summary-card">
        <div class="sc-icon red"><i class="fa-solid fa-arrow-trend-down"></i></div>
        <div class="sc-body">
          <div class="sc-label">Total de Saídas</div>
          <div class="sc-value red">${brl(totalSaidas)}</div>
        </div>
      </div>
    </div>

    <div class="card" style="margin-bottom:24px">
      <div style="font-weight:700;font-size:.95rem;margin-bottom:12px">
        <i class="fa-solid fa-cash-register" style="color:var(--primary);margin-right:6px"></i>Resumo por Método
      </div>
      <div class="tabs" style="margin-bottom:12px">
        <button class="tab-btn ${_caixaTab==='entradas'?'active':''}" data-caixa-tab="entradas" onclick="setCaixaTab('entradas')">
          <i class="fa-solid fa-arrow-trend-up"></i> Entradas
        </button>
        <button class="tab-btn ${_caixaTab==='saidas'?'active':''}" data-caixa-tab="saidas" onclick="setCaixaTab('saidas')">
          <i class="fa-solid fa-arrow-trend-down"></i> Saídas
        </button>
      </div>
      <div id="resumo-metodo-list" class="method-summary-list">
        ${buildResumoPorMetodo(_caixaTab) || '<p style="color:var(--text-muted);font-size:.875rem">Nenhuma transação nesta categoria.</p>'}
      </div>
    </div>

    <div style="font-weight:700;font-size:1rem;margin-bottom:16px">
      <i class="fa-solid fa-wallet" style="color:var(--primary);margin-right:6px"></i>Saldo por Método de Pagamento
    </div>

    ${methodCards || `<div class="empty-state">
      <div class="empty-icon"><i class="fa-solid fa-wallet"></i></div>
      <p>Nenhuma transação registrada ainda.</p>
    </div>`}
  `);

  window._caixaGroups = groups;
}

function setCaixaTab(tab) {
  _caixaTab = tab;
  document.querySelectorAll('[data-caixa-tab]').forEach(b => {
    b.classList.toggle('active', b.dataset.caixaTab === tab);
  });

  const groups = window._caixaGroups || {};
  const html = Object.entries(groups).map(([key, g]) => {
    const items = tab === 'entradas' ? g.entradas : g.saidas;
    const total = tab === 'entradas'
      ? items.reduce((s, r) => s + (r.amount || 0), 0)
      : items.reduce((s, e) => s + (e.value || 0), 0);
    if (total === 0) return '';
    return `<div class="method-summary-item">
      <div class="method-summary-icon"><i class="fa-solid ${FORMA_PAG_ICON[key] || 'fa-wallet'}"></i></div>
      <span class="method-summary-name">${FORMA_PAG[key] || '—'}</span>
      <span class="method-summary-value ${tab === 'saidas' ? 'out' : ''}">${brl(total)}</span>
      <i class="fa-solid fa-chevron-right" style="color:var(--text-muted);font-size:.75rem"></i>
    </div>`;
  }).join('');

  document.getElementById('resumo-metodo-list').innerHTML =
    html || '<p style="color:var(--text-muted);font-size:.875rem">Nenhuma transação nesta categoria.</p>';
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
          ${list.length === 0
            ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-clipboard-list"></i></div><p>Nenhum pedido</p></div></td></tr>`
            : list.map(o => `
              <tr>
                <td><strong>${o.name || '—'}</strong></td>
                <td>${fmtDate(o.eventDate)}</td>
                <td>${brl(o.sinal)}</td>
                <td>${brl(o.totalValue)}</td>
                <td><span class="badge ${STATUS_CLASS[o.status]}">${STATUS_ORDER[o.status]}</span></td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='viewPedido(${JSON.stringify(o).replace(/'/g,"&#39;")})' title="Ver"><i class="fa-solid fa-eye"></i></button>
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editPedido(${JSON.stringify(o).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
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
        <button class="btn btn-sm btn-secondary btn-icon" onclick='viewPedido(${JSON.stringify(o).replace(/'/g,"&#39;")})' title="Ver"><i class="fa-solid fa-eye"></i></button>
        <button class="btn btn-sm btn-secondary btn-icon" onclick='editPedido(${JSON.stringify(o).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
        <button class="btn btn-sm btn-danger btn-icon" onclick="deletePedido('${o.id}')"><i class="fa-solid fa-trash"></i></button>
      </div></td>
    </tr>`).join('')
    || `<tr><td colspan="6"><div class="empty-state"><p>Nenhum resultado</p></div></td></tr>`;
}

function filterPedidoStatus(val) {
  const f = val === '' ? pedidosData : pedidosData.filter(o => String(o.status) === val);
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

function btnNewPedido() { openModal('Novo Pedido', formPedido(null)); }
function editPedido(o)  { openModal('Editar Pedido', formPedido(o)); }

function formPedido(o) {
  const statusOpts = Object.entries(STATUS_ORDER).map(([k, val]) =>
    `<option value="${k}" ${o && o.status == k ? 'selected' : ''}>${val}</option>`).join('');
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
    name: v('p-name'),
    eventDate: v('p-date') || null,
    status: parseInt(v('p-status')),
    sinal: parseFloat(v('p-sinal')) || null,
    totalValue: parseFloat(v('p-total')) || null,
    items: []
  };
  const r = await post('/Orders/Create', body);
  if (r.isSuccessful) { closeModal(); toast('Pedido criado!'); renderPedidos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function savePedido(id) {
  const body = {
    name: v('p-name') || null,
    eventDate: v('p-date') || null,
    status: parseInt(v('p-status')),
    sinal: parseFloat(v('p-sinal')) || null,
    totalValue: parseFloat(v('p-total')) || null,
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
      <button class="btn btn-primary" onclick="btnNewOrcamento()"><i class="fa-solid fa-plus"></i> Novo Orçamento</button>
    </div>
    <div class="filters">
      <input class="search-input" placeholder="Buscar orçamento..." oninput="filterOrcamentos(this.value)" />
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Cliente</th><th>Produto</th><th>Data Evento</th><th>Total</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0
            ? `<tr><td colspan="5"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-file-invoice"></i></div><p>Nenhum orçamento</p></div></td></tr>`
            : list.map(b => `
              <tr>
                <td><strong>${b.clientName||'—'}</strong></td>
                <td>${trunc(b.finalProductName)}</td>
                <td>${fmtDate(b.eventDate)}</td>
                <td>${brl(b.finalTotalValue)}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='viewOrcamento(${JSON.stringify(b).replace(/'/g,"&#39;")})' title="Ver"><i class="fa-solid fa-eye"></i></button>
                  <button class="btn btn-sm btn-success btn-icon" onclick="convertToOrder('${b.id}')" title="Converter em Pedido"><i class="fa-solid fa-arrows-rotate"></i></button>
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editOrcamento(${JSON.stringify(b).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteOrcamento('${b.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function filterOrcamentos(q) {
  const f = orcamentosData.filter(b =>
    (b.clientName||'').toLowerCase().includes(q.toLowerCase()) ||
    (b.finalProductName||'').toLowerCase().includes(q.toLowerCase()));
  document.querySelector('#content .table-wrap tbody').innerHTML =
    f.map(b => `
      <tr>
        <td><strong>${b.clientName||'—'}</strong></td>
        <td>${trunc(b.finalProductName)}</td>
        <td>${fmtDate(b.eventDate)}</td>
        <td>${brl(b.finalTotalValue)}</td>
        <td><div class="actions">
          <button class="btn btn-sm btn-secondary btn-icon" onclick='viewOrcamento(${JSON.stringify(b).replace(/'/g,"&#39;")})' title="Ver"><i class="fa-solid fa-eye"></i></button>
          <button class="btn btn-sm btn-success btn-icon" onclick="convertToOrder('${b.id}')" title="Converter"><i class="fa-solid fa-arrows-rotate"></i></button>
          <button class="btn btn-sm btn-secondary btn-icon" onclick='editOrcamento(${JSON.stringify(b).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
          <button class="btn btn-sm btn-danger btn-icon" onclick="deleteOrcamento('${b.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
        </div></td>
      </tr>`).join('')
    || `<tr><td colspan="5"><div class="empty-state"><p>Nenhum resultado</p></div></td></tr>`;
}

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
        <input class="form-control" id="b-desc" value="${b?.finalProductDescription||''}" />
      </div>
      <div class="form-row">
        <div class="form-group">
          <label>Preço Unitário (R$)</label>
          <input class="form-control" id="b-unit" type="number" step="0.01" value="${b?.finalUnitPrice||''}" />
        </div>
        <div class="form-group">
          <label>Total (R$)</label>
          <input class="form-control" id="b-total" type="number" step="0.01" value="${b?.finalTotalValue||''}" />
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
    clientName: v('b-client') || null,
    eventDate: v('b-date') || null,
    finalProductName: v('b-prod') || null,
    finalProductDescription: v('b-desc') || null,
    finalUnitPrice: parseFloat(v('b-unit')) || null,
    finalTotalValue: parseFloat(v('b-total')) || null,
    items: []
  };
  const r = await post('/Budgets/Create', body);
  if (r.isSuccessful) { closeModal(); toast('Orçamento criado!'); renderOrcamentos(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function saveOrcamento(id) {
  const body = {
    clientName: v('b-client') || null,
    eventDate: v('b-date') || null,
    finalProductName: v('b-prod') || null,
    finalProductDescription: v('b-desc') || null,
    finalUnitPrice: parseFloat(v('b-unit')) || null,
    finalTotalValue: parseFloat(v('b-total')) || null,
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
//  CLIENTES
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
      newFn: 'btnNewCliente',
      editFn: 'editCliente',
      deleteFn: 'deleteCliente',
    },
  };
  const cfg = configs[module];
  if (!cfg) return;
  setContent(loadingHtml());
  const res = await get(`/${cfg.endpoint}/GetAll?page=1&pageSize=1000`);
  const list = res.data || [];
  setContent(`
    <div class="page-header">
      <h1>${cfg.icon} ${cfg.title}</h1>
      <button class="btn btn-primary" onclick="${cfg.newFn}()">${cfg.icon} ${cfg.newLabel}</button>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr>${cfg.cols.map(c=>`<th>${c}</th>`).join('')}<th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0
            ? `<tr><td colspan="${cfg.cols.length+1}"><div class="empty-state"><div class="empty-icon">${cfg.icon}</div><p>Nenhum registro</p></div></td></tr>`
            : list.map(item => `
              <tr>
                ${cfg.row(item).map(val=>`<td>${val}</td>`).join('')}
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='${cfg.editFn}(${JSON.stringify(item).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="${cfg.deleteFn}('${item.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>
  `);
}

function btnNewCliente()  { openModal('Novo Cliente', formCliente(null)); }
function editCliente(c)   { openModal('Editar Cliente', formCliente(c)); }

function formCliente(c) {
  return `<div class="form-grid">
    <div class="form-row">
      <div class="form-group"><label>Nome *</label>
        <input class="form-control" id="c-name" value="${c?.name||''}" required /></div>
      <div class="form-group"><label>Telefone</label>
        <input class="form-control" id="c-phone" value="${c?.phone||''}" placeholder="(11) 99999-0000" /></div>
    </div>
    <div class="form-group"><label>Email</label>
      <input class="form-control" id="c-email" type="email" value="${c?.email||''}" /></div>
    <div class="form-group"><label>Endereço</label>
      <input class="form-control" id="c-addr" value="${c?.address||''}" /></div>
    <div class="form-group"><label>Nascimento</label>
      <input class="form-control" id="c-birth" type="date" value="${c?.birthDate ? c.birthDate.slice(0,10) : ''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${c ? `saveCliente('${c.id}')` : 'createCliente()'}">Salvar</button>
    </div>
  </div>`;
}

async function createCliente() {
  const b = { name: v('c-name'), phone: v('c-phone')||null, email: v('c-email')||null, address: v('c-addr')||null, birthDate: v('c-birth')||null };
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

// ═══════════════════════════════════════════════════════
//  ESTOQUE (Insumos + Produtos Finais + Categorias + Movimentações)
// ═══════════════════════════════════════════════════════
let estoqueTab = 'supplies';

async function renderEstoque() {
  setContent(loadingHtml());
  const [inv, finals] = await Promise.all([
    get('/Inventories/GetInventory'),
    get('/Inventories/GetFinalProducts?page=1&pageSize=1000'),
  ]);
  const supplies  = inv.data?.supplies || [];
  const products  = finals.data || [];
  const totalInv  = inv.data?.totalInvested || 0;

  setContent(`
    <div class="page-header">
      <h1><i class="fa-solid fa-boxes-stacked" style="color:var(--primary);margin-right:8px"></i>Estoque</h1>
    </div>
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
      <button class="tab-btn ${estoqueTab==='supplies'?'active':''}" onclick="switchEstoqueTab('supplies')">
        <i class="fa-solid fa-boxes-stacked"></i> Insumos
      </button>
      <button class="tab-btn ${estoqueTab==='finals'?'active':''}" onclick="switchEstoqueTab('finals')">
        <i class="fa-solid fa-cake-candles"></i> Produtos Finais
      </button>
      <button class="tab-btn ${estoqueTab==='categorias'?'active':''}" onclick="switchEstoqueTab('categorias')">
        <i class="fa-solid fa-tags"></i> Categorias
      </button>
      <button class="tab-btn ${estoqueTab==='movimentacoes'?'active':''}" onclick="switchEstoqueTab('movimentacoes')">
        <i class="fa-solid fa-arrow-right-arrow-left"></i> Movimentações
      </button>
    </div>

    <div id="estoque-content">
      ${estoqueTab === 'supplies'     ? buildSuppliesTable(supplies)  : ''}
      ${estoqueTab === 'finals'       ? buildFinalsTable(products)    : ''}
      ${estoqueTab === 'categorias'   ? loadingHtml()                 : ''}
      ${estoqueTab === 'movimentacoes'? loadingHtml()                 : ''}
    </div>
  `);

  window._estoque = { supplies, products };

  if (estoqueTab === 'categorias')   loadEstoqueCategorias();
  if (estoqueTab === 'movimentacoes') loadEstoqueMovimentacoes();
}

async function switchEstoqueTab(tab) {
  estoqueTab = tab;
  const tabMap = { supplies: 0, finals: 1, categorias: 2, movimentacoes: 3 };
  document.querySelectorAll('.tabs .tab-btn').forEach((b, i) => {
    b.classList.toggle('active', tabMap[tab] === i);
  });
  const d = document.getElementById('estoque-content');
  if (tab === 'supplies')      d.innerHTML = buildSuppliesTable(window._estoque?.supplies || []);
  else if (tab === 'finals')   d.innerHTML = buildFinalsTable(window._estoque?.products || []);
  else if (tab === 'categorias')    { d.innerHTML = loadingHtml(); loadEstoqueCategorias(); }
  else if (tab === 'movimentacoes') { d.innerHTML = loadingHtml(); loadEstoqueMovimentacoes(); }
}

async function loadEstoqueCategorias() {
  const res = await get('/Categories/GetAll?page=1&pageSize=1000');
  const list = res.data || [];
  document.getElementById('estoque-content').innerHTML = `
    <div class="section-header" style="margin-top:16px">
      <h2>Categorias</h2>
      <button class="btn btn-primary btn-sm" onclick="btnNewCategoria()">
        <i class="fa-solid fa-plus"></i> Nova Categoria
      </button>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Nome</th><th>Descrição</th><th>Ações</th></tr></thead>
        <tbody>
          ${list.length === 0
            ? `<tr><td colspan="3"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-tags"></i></div><p>Nenhuma categoria</p></div></td></tr>`
            : list.map(c => `
              <tr>
                <td><strong>${c.name||'—'}</strong></td>
                <td>${trunc(c.description, 40)}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editCategoria(${JSON.stringify(c).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteCategoria('${c.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>`;
}

async function loadEstoqueMovimentacoes() {
  const res = await get('/StockMovements/GetAll?page=1&pageSize=1000');
  const list = res.data || [];
  const entradas = list.filter(m => m.type === 1).reduce((s,m) => s + (m.quantity||0), 0);
  const saidas   = list.filter(m => m.type === 0).reduce((s,m) => s + (m.quantity||0), 0);
  document.getElementById('estoque-content').innerHTML = `
    <div class="section-header" style="margin-top:16px">
      <h2>Movimentações de Estoque</h2>
      <button class="btn btn-primary btn-sm" onclick="btnNewMovimento()">
        <i class="fa-solid fa-plus"></i> Registrar
      </button>
    </div>
    <div class="cards-grid" style="margin-bottom:16px">
      <div class="card"><div class="card-icon"><i class="fa-solid fa-arrow-up" style="color:var(--success)"></i></div><div class="card-label">Entradas</div><div class="card-value green">${entradas}</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-arrow-down" style="color:var(--danger)"></i></div><div class="card-label">Saídas</div><div class="card-value red">${saidas}</div></div>
      <div class="card"><div class="card-icon"><i class="fa-solid fa-hashtag" style="color:var(--primary)"></i></div><div class="card-label">Registros</div><div class="card-value pink">${list.length}</div></div>
    </div>
    <div class="table-wrap">
      <table class="data-table">
        <thead><tr><th>Data</th><th>Tipo</th><th>Qtd</th><th>Notas</th></tr></thead>
        <tbody>
          ${list.length === 0
            ? `<tr><td colspan="4"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-arrow-right-arrow-left"></i></div><p>Nenhuma movimentação</p></div></td></tr>`
            : [...list].reverse().map(m => `
              <tr>
                <td>${fmtDatetime(m.date)}</td>
                <td><span class="badge ${m.type === 1 ? 'badge-green' : 'badge-red'}">${MOV_TYPE[m.type]||'—'}</span></td>
                <td>${m.quantity}</td>
                <td>${m.notes||'—'}</td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>`;
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
          ${list.length === 0
            ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-boxes-stacked"></i></div><p>Nenhum insumo</p></div></td></tr>`
            : list.map(s => `
              <tr>
                <td><strong>${s.name||'—'}</strong></td>
                <td>${s.quantity??'—'}</td>
                <td><span class="badge badge-gray">${UNIDADE[s.unit]||s.unit}</span></td>
                <td>${brl(s.price)}</td>
                <td>${brl(s.totalPrice)}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editSupply(${JSON.stringify(s).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteSupply('${s.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>`;
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
          ${list.length === 0
            ? `<tr><td colspan="6"><div class="empty-state"><div class="empty-icon"><i class="fa-solid fa-cake-candles"></i></div><p>Nenhum produto final</p></div></td></tr>`
            : list.map(p => `
              <tr>
                <td><strong>${p.name||'—'}</strong></td>
                <td>${trunc(p.description, 25)}</td>
                <td>${brl(p.costPrice)}</td>
                <td>${brl(p.unitPrice)}</td>
                <td>${p.quantityAvailable??'—'}</td>
                <td><div class="actions">
                  <button class="btn btn-sm btn-secondary btn-icon" onclick='editFinal(${JSON.stringify(p).replace(/'/g,"&#39;")})' title="Editar"><i class="fa-solid fa-pencil"></i></button>
                  <button class="btn btn-sm btn-danger btn-icon" onclick="deleteFinal('${p.id}')" title="Excluir"><i class="fa-solid fa-trash"></i></button>
                </div></td>
              </tr>`).join('')}
        </tbody>
      </table>
    </div>`;
}

// -- Insumos --
function btnNewSupply() { openModal('Novo Insumo', formSupply(null)); }
function editSupply(s)  { openModal('Editar Insumo', formSupply(s)); }

function formSupply(s) {
  const unitOpts = Object.entries(UNIDADE).map(([k, val]) =>
    `<option value="${k}" ${s && s.unit == k ? 'selected' : ''}>${val}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label>
      <input class="form-control" id="s-name" value="${s?.name||''}" required /></div>
    <div class="form-row">
      <div class="form-group"><label>Quantidade</label>
        <input class="form-control" id="s-qty" type="number" step="0.001" value="${s?.quantity||''}" /></div>
      <div class="form-group"><label>Unidade</label>
        <select class="form-control" id="s-unit">${unitOpts}</select></div>
    </div>
    <div class="form-group"><label>Preço Unitário (R$)</label>
      <input class="form-control" id="s-price" type="number" step="0.01" value="${s?.price||''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${s ? `saveSupply('${s.id}')` : 'createSupply()'}">Salvar</button>
    </div>
  </div>`;
}

async function createSupply() {
  const b = { name: v('s-name'), quantity: parseFloat(v('s-qty'))||null, unit: parseInt(v('s-unit')), price: parseFloat(v('s-price'))||null };
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
function btnNewFinal() { openModal('Novo Produto Final', formFinal(null)); }
function editFinal(p)  { openModal('Editar Produto Final', formFinal(p)); }

function formFinal(p) {
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label>
      <input class="form-control" id="f-name" value="${p?.name||''}" required /></div>
    <div class="form-group"><label>Descrição</label>
      <input class="form-control" id="f-desc" value="${p?.description||''}" /></div>
    <div class="form-row">
      <div class="form-group"><label>Preço de Custo (R$)</label>
        <input class="form-control" id="f-cost" type="number" step="0.01" value="${p?.costPrice||''}" /></div>
      <div class="form-group"><label>Preço de Venda (R$)</label>
        <input class="form-control" id="f-price" type="number" step="0.01" value="${p?.unitPrice||''}" /></div>
    </div>
    <div class="form-group"><label>Quantidade Disponível</label>
      <input class="form-control" id="f-qty" type="number" step="0.01" value="${p?.quantityAvailable||''}" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${p ? `saveFinal('${p.id}')` : 'createFinal()'}">Salvar</button>
    </div>
  </div>`;
}

async function createFinal() {
  const b = { name: v('f-name'), description: v('f-desc')||null, costPrice: parseFloat(v('f-cost'))||null, unitPrice: parseFloat(v('f-price'))||null, quantityAvailable: parseFloat(v('f-qty'))||null };
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

// -- Categorias --
function btnNewCategoria() { openModal('Nova Categoria', formCategoria(null)); }
function editCategoria(c)  { openModal('Editar Categoria', formCategoria(c)); }

function formCategoria(c) {
  return `<div class="form-grid">
    <div class="form-group"><label>Nome *</label>
      <input class="form-control" id="cat-name" value="${c?.name||''}" required /></div>
    <div class="form-group"><label>Descrição</label>
      <textarea class="form-control" id="cat-desc" rows="2">${c?.description||''}</textarea></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="${c ? `saveCategoria('${c.id}')` : 'createCategoria()'}">Salvar</button>
    </div>
  </div>`;
}

async function createCategoria() {
  const b = { name: v('cat-name'), description: v('cat-desc')||null };
  const r = await post('/Categories/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Categoria criada!'); loadEstoqueCategorias(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function saveCategoria(id) {
  const b = { name: v('cat-name')||null, description: v('cat-desc')||null };
  const r = await put(`/Categories/Update/${id}`, b);
  if (r.isSuccessful) { closeModal(); toast('Categoria atualizada!'); loadEstoqueCategorias(); }
  else toast((r.messages||[]).join(', '), 'error');
}

async function deleteCategoria(id) {
  if (!confirm('Excluir esta categoria?')) return;
  const r = await del(`/Categories/Delete/${id}`);
  if (r.isSuccessful) { toast('Categoria excluída!'); loadEstoqueCategorias(); }
  else toast((r.messages||[]).join(', '), 'error');
}

// -- Movimentações --
function btnNewMovimento() { openModal('Registrar Movimentação', formMovimento()); }

function formMovimento() {
  const typeOpts = Object.entries(MOV_TYPE).map(([k, val]) =>
    `<option value="${k}" ${k == '1' ? 'selected' : ''}>${val}</option>`).join('');
  return `<div class="form-grid">
    <div class="form-group"><label>ID do Insumo</label>
      <input class="form-control" id="m-supply" placeholder="GUID do insumo" /></div>
    <div class="form-row">
      <div class="form-group"><label>Quantidade *</label>
        <input class="form-control" id="m-qty" type="number" step="0.001" required /></div>
      <div class="form-group"><label>Tipo</label>
        <select class="form-control" id="m-type">${typeOpts}</select></div>
    </div>
    <div class="form-group"><label>Notas</label>
      <input class="form-control" id="m-notes" placeholder="Ex: Compra de farinha de trigo" /></div>
    <div class="modal-footer">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="createMovimento()">Salvar</button>
    </div>
  </div>`;
}

async function createMovimento() {
  const b = {
    supplyId: v('m-supply') || null,
    quantity: parseFloat(v('m-qty')) || 0,
    type: parseInt(v('m-type')),
    notes: v('m-notes') || null,
  };
  const r = await post('/StockMovements/Create', b);
  if (r.isSuccessful) { closeModal(); toast('Movimentação registrada!'); loadEstoqueMovimentacoes(); }
  else toast((r.messages||[]).join(', '), 'error');
}
