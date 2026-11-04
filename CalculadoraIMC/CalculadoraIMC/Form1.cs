using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace CalculadoraIMC
{
    public partial class Form1 : Form
    {
        // ===== Controlos pedidos no enunciado =====
        private readonly TextBox txtPeso = new TextBox { Name = "txtPeso" };
        private readonly TextBox txtAltura = new TextBox { Name = "txtAltura" };
        private readonly GradientButton btnCalcular = new GradientButton { Name = "btnCalcular" };
        private readonly Label lblResultado = new Label { Name = "lblResultado" };

        private RoundedPanel caixaResultado;

        // ===== Paleta: índigo + violeta com acento coral =====
        private static readonly Color Indigo = Color.FromArgb(49, 46, 129);
        private static readonly Color Violeta = Color.FromArgb(109, 40, 217);
        private static readonly Color VioletaClaro = Color.FromArgb(124, 58, 237);
        private static readonly Color Lavanda = Color.FromArgb(221, 214, 254);
        private static readonly Color Fundo = Color.FromArgb(248, 246, 255);
        private static readonly Color CampoFundo = Color.FromArgb(245, 243, 255);
        private static readonly Color CampoBorda = Color.FromArgb(216, 208, 246);
        private static readonly Color TextoEscuro = Color.FromArgb(30, 27, 75);
        private static readonly Color TextoSuave = Color.FromArgb(107, 100, 150);
        private static readonly Color RodapeFundo = Color.FromArgb(30, 27, 75);

        // ===== Tipografia =====
        private static readonly Font FonteRotulo = new Font("Segoe UI Semibold", 11F);
        private static readonly Font FonteCampo = new Font("Segoe UI", 13F);
        private static readonly Font FonteBotao = new Font("Segoe UI Semibold", 16F);
        private static readonly Font FonteResultado = new Font("Segoe UI Semibold", 13F);
        private static readonly Font FonteMensagem = new Font("Segoe UI", 9.5F);
        private static readonly Font FonteNota = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        private static readonly Font FonteRodape = new Font("Segoe UI", 9F);

        public Form1()
        {
            InitializeComponent();
            ConstruirInterface();

            btnCalcular.Click += btnCalcular_Click;
            AcceptButton = btnCalcular; // Enter também calcula
        }

        // =====================================================
        //  LÓGICA
        // =====================================================
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            // 1. Conversão segura (nunca lança exceção com texto inválido)
            if (!TentarConverter(txtPeso.Text, out double peso) || peso <= 0)
            {
                MostrarErro("Peso inválido. Introduza um número positivo (ex: 75.5).");
                txtPeso.Focus();
                return;
            }

            if (!TentarConverter(txtAltura.Text, out double altura) || altura <= 0)
            {
                MostrarErro("Altura inválida. Introduza um número positivo em metros (ex: 1.75).");
                txtAltura.Focus();
                return;
            }

            // 2. Fórmula padrão: IMC = peso / altura²
            double imc = peso / (altura * altura);

            // 3. Resultado e classificação (com cor correspondente)
            string classificacao = Classificar(imc, out Color cor);

            lblResultado.Font = FonteResultado;
            lblResultado.ForeColor = cor;
            lblResultado.Text = $"IMC: {imc:F2}\n{classificacao}";
            caixaResultado.AcentoLateral = cor;
        }

        private static bool TentarConverter(string texto, out double valor)
        {
            // Aceita ponto ou vírgula; cultura invariante para não depender do Windows
            string normalizado = (texto ?? string.Empty).Trim().Replace(',', '.');

            return double.TryParse(
                normalizado,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out valor);
        }

        private static string Classificar(double imc, out Color cor)
        {
            if (imc < 18.5) { cor = Color.FromArgb(37, 99, 235); return "Abaixo do peso"; }
            if (imc < 25.0) { cor = Color.FromArgb(22, 163, 74); return "Peso normal"; }
            if (imc < 30.0) { cor = Color.FromArgb(202, 138, 4); return "Sobrepeso"; }
            if (imc < 35.0) { cor = Color.FromArgb(234, 88, 12); return "Obesidade grau I"; }
            if (imc < 40.0) { cor = Color.FromArgb(220, 38, 38); return "Obesidade grau II"; }
            cor = Color.FromArgb(153, 27, 27);
            return "Obesidade grau III";
        }

        private void MostrarErro(string mensagem)
        {
            lblResultado.Font = FonteMensagem;
            lblResultado.ForeColor = Color.FromArgb(190, 18, 60);
            lblResultado.Text = mensagem;
            caixaResultado.AcentoLateral = Color.FromArgb(190, 18, 60);
        }

        // =====================================================
        //  INTERFACE (construída em código)
        // =====================================================
        private void ConstruirInterface()
        {
            Text = "Calculadora de IMC";
            ClientSize = new Size(820, 520);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Fundo;
            Font = new Font("Segoe UI", 10F);

            // --- Cabeçalho com gradiente ---
            var cabecalho = new HeaderPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                Titulo = "Calculadora de IMC",
                Subtitulo = "Índice de Massa Corporal",
                Cor1 = Indigo,
                Cor2 = Violeta
            };

            // --- Rodapé ---
            var rodape = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = RodapeFundo };
            rodape.Controls.Add(new Label
            {
                Text = "Desenvolvido por: Simao Baptista Buque",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(196, 181, 253),
                Font = FonteRodape
            });

            // --- Cartão central ---
            var cartao = new RoundedPanel(28, Color.White, Color.FromArgb(233, 228, 250))
            {
                Location = new Point(100, 145),
                Size = new Size(620, 320)
            };

            cartao.Controls.Add(CriarRotulo("Peso (kg)", 30, 18));
            cartao.Controls.Add(CriarCampo(txtPeso, "Digite o seu peso...", 30, 48));

            cartao.Controls.Add(CriarRotulo("Altura (m)", 30, 108));
            cartao.Controls.Add(CriarCampo(txtAltura, "Digite a sua altura (ex: 1.75)...", 30, 138));

            // Botão com gradiente coral
            btnCalcular.Text = "Calcular IMC";
            btnCalcular.Font = FonteBotao;
            btnCalcular.ForeColor = Color.White;
            btnCalcular.Cor1 = Color.FromArgb(236, 72, 153);
            btnCalcular.Cor2 = Color.FromArgb(249, 115, 22);
            btnCalcular.Location = new Point(30, 208);
            btnCalcular.Size = new Size(210, 84);
            cartao.Controls.Add(btnCalcular);

            // Caixa do resultado (com barra lateral colorida)
            caixaResultado = new RoundedPanel(16, CampoFundo, CampoFundo)
            {
                Location = new Point(260, 208),
                Size = new Size(330, 72),
                AcentoLateral = Lavanda
            };
            lblResultado.AutoSize = false;
            lblResultado.Location = new Point(28, 4);
            lblResultado.Size = new Size(caixaResultado.Width - 38, caixaResultado.Height - 8);
            lblResultado.TextAlign = ContentAlignment.MiddleLeft;
            lblResultado.BackColor = CampoFundo;
            lblResultado.ForeColor = TextoSuave;
            lblResultado.Font = FonteMensagem;
            lblResultado.Text = "Resultado do IMC e Classificação aparecerão aqui.";
            caixaResultado.Controls.Add(lblResultado);
            cartao.Controls.Add(caixaResultado);

            // Nota
            cartao.Controls.Add(new Label
            {
                Text = "Use ponto para decimais (ex: 1.75 e 75.5)",
                Location = new Point(262, 288),
                AutoSize = true,
                ForeColor = TextoSuave,
                Font = FonteNota
            });

            Controls.Add(cartao);
            Controls.Add(rodape);
            Controls.Add(cabecalho);
        }

        private static Label CriarRotulo(string texto, int x, int y)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = TextoEscuro,
                Font = FonteRotulo
            };
        }

        private static RoundedPanel CriarCampo(TextBox txt, string placeholder, int x, int y)
        {
            var painel = new RoundedPanel(22, CampoFundo, CampoBorda)
            {
                Location = new Point(x, y),
                Size = new Size(560, 46)
            };

            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = CampoFundo;
            txt.ForeColor = TextoEscuro;
            txt.Font = FonteCampo;
            txt.PlaceholderText = placeholder;
            txt.Width = painel.Width - 40;
            txt.Location = new Point(20, (painel.Height - txt.Height) / 2);

            // Realce da borda quando o campo tem o foco
            txt.Enter += (s, e) => painel.CorBorda = VioletaClaro;
            txt.Leave += (s, e) => painel.CorBorda = CampoBorda;

            painel.Controls.Add(txt);
            return painel;
        }
    }

    /// <summary>Cabeçalho com gradiente, círculos decorativos, título e subtítulo.</summary>
    public class HeaderPanel : Panel
    {
        public string Titulo { get; set; } = "";
        public string Subtitulo { get; set; } = "";
        public Color Cor1 { get; set; } = Color.Indigo;
        public Color Cor2 { get; set; } = Color.BlueViolet;

        public HeaderPanel()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 1 || Height < 1) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (var gradiente = new LinearGradientBrush(ClientRectangle, Cor1, Cor2, 20f))
                g.FillRectangle(gradiente, ClientRectangle);

            // Círculos decorativos translúcidos
            using (var circulo = new SolidBrush(Color.FromArgb(28, 255, 255, 255)))
            {
                g.FillEllipse(circulo, Width - 200, -80, 260, 260);
                g.FillEllipse(circulo, -60, 60, 170, 170);
                g.FillEllipse(circulo, Width - 330, 70, 90, 90);
            }

            var centro = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (var fonteTitulo = new Font("Georgia", 30F, FontStyle.Bold))
                g.DrawString(Titulo, fonteTitulo, Brushes.White,
                             new RectangleF(0, 20, Width, 62), centro);

            using (var fonteSub = new Font("Segoe UI", 11.5F))
            using (var pincelSub = new SolidBrush(Color.FromArgb(221, 214, 254)))
                g.DrawString(Subtitulo.ToUpper(), fonteSub, pincelSub,
                             new RectangleF(0, 82, Width, 28), centro);
        }
    }

    /// <summary>Botão com gradiente, cantos arredondados e efeito hover/pressionado.</summary>
    public class GradientButton : Button
    {
        public Color Cor1 { get; set; } = Color.HotPink;
        public Color Cor2 { get; set; } = Color.Orange;

        private bool _hover;
        private bool _pressionado;

        public GradientButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
                Region = new Region(RoundedPanel.CriarCaminho(new Rectangle(0, 0, Width, Height), 26));
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressionado = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _pressionado = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _pressionado = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 1 || Height < 1) return;

            Color c1 = Cor1, c2 = Cor2;
            if (_pressionado) { c1 = ControlPaint.Dark(c1, 0.1f); c2 = ControlPaint.Dark(c2, 0.1f); }
            else if (_hover) { c1 = ControlPaint.Light(c1, 0.25f); c2 = ControlPaint.Light(c2, 0.25f); }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var gradiente = new LinearGradientBrush(ClientRectangle, c1, c2, 25f))
                e.Graphics.FillRectangle(gradiente, ClientRectangle);

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }

    /// <summary>Painel com cantos arredondados, borda e barra lateral opcional.</summary>
    public class RoundedPanel : Panel
    {
        private readonly int _raio;
        private readonly Color _preenchimento;
        private Color _corBorda;
        private Color _acentoLateral = Color.Empty;

        public Color CorBorda
        {
            get { return _corBorda; }
            set { _corBorda = value; Invalidate(); }
        }

        public Color AcentoLateral
        {
            get { return _acentoLateral; }
            set { _acentoLateral = value; Invalidate(); }
        }

        public RoundedPanel(int raio, Color preenchimento, Color borda)
        {
            _raio = raio;
            _preenchimento = preenchimento;
            _corBorda = borda;

            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var area = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var caminho = CriarCaminho(area, _raio))
            using (var pincel = new SolidBrush(_preenchimento))
            using (var caneta = new Pen(_corBorda, 2))
            {
                e.Graphics.FillPath(pincel, caminho);
                e.Graphics.DrawPath(caneta, caminho);
            }

            if (_acentoLateral != Color.Empty)
            {
                var barra = new Rectangle(12, 14, 6, Height - 29);
                using (var caminhoBarra = CriarCaminho(barra, 3))
                using (var pincelBarra = new SolidBrush(_acentoLateral))
                    e.Graphics.FillPath(pincelBarra, caminhoBarra);
            }
        }

        public static GraphicsPath CriarCaminho(Rectangle r, int raio)
        {
            int d = raio * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}