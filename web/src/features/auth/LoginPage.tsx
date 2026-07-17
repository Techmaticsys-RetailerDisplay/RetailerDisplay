import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext";
import { apiError } from "../../api/client";
import { Button, TextInput, ErrorText } from "../../components/ui";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setBusy(true);
    try {
      await login(email, password);
      navigate("/");
    } catch (err) {
      setError(apiError(err));
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="auth-split">
      <section className="auth-hero">
        <div className="brand">
          <span className="dot">▦</span>
          <span>Retailer<span className="accent">Display</span></span>
        </div>
        <h2>Your screens, your content — everywhere.</h2>
        <p>Upload images and videos, build playlists, and push them to every TV and tablet in your stores from one place.</p>
        <SignageScene />
      </section>

      <section className="auth-panel">
        <form className="auth-card" onSubmit={submit}>
          <div className="auth-brand">
            <span className="dot">▦</span>
            <span>Retailer<span className="accent">Display</span></span>
          </div>
          <p className="auth-sub">Sign in to your account</p>
          <div className="stack">
            <TextInput value={email} onChange={setEmail} placeholder="Email" type="email" />
            <TextInput value={password} onChange={setPassword} placeholder="Password" type="password" />
            <Button type="submit" disabled={busy}>{busy ? "Signing in…" : "Sign in"}</Button>
            <ErrorText>{error}</ErrorText>
          </div>
        </form>
      </section>
    </div>
  );
}

/** Self-contained illustration: a wall TV and a tablet displaying content. */
function SignageScene() {
  const tile = (x: number, y: number, w: number, h: number, fill: string, glyph?: "sun" | "hills") => (
    <g>
      <rect x={x} y={y} width={w} height={h} rx="6" fill={fill} />
      {glyph === "sun" && <circle cx={x + w - 16} cy={y + 15} r="7" fill="#fff" opacity="0.9" />}
      {glyph === "hills" && (
        <path d={`M${x} ${y + h} L${x + w * 0.4} ${y + h * 0.45} L${x + w * 0.62} ${y + h * 0.7} L${x + w * 0.82} ${y + h * 0.5} L${x + w} ${y + h} Z`} fill="#fff" opacity="0.85" />
      )}
    </g>
  );

  return (
    <svg className="scene" viewBox="0 0 560 340" xmlns="http://www.w3.org/2000/svg" role="img" aria-label="Screens displaying content">
      {/* TV */}
      <rect x="20" y="20" width="372" height="228" rx="14" fill="#0d1420" stroke="#31405a" strokeWidth="2" />
      {/* content grid inside TV */}
      {tile(40, 40, 104, 90, "#f2a13d", "sun")}
      {tile(154, 40, 104, 90, "#3ec7c0", "hills")}
      {tile(268, 40, 104, 90, "#ff6fa0")}
      {tile(40, 138, 104, 90, "#a58cff")}
      {tile(154, 138, 104, 90, "#6fa0ff", "hills")}
      {tile(268, 138, 104, 90, "#43c47e")}
      {/* TV stand */}
      <rect x="196" y="248" width="20" height="26" fill="#31405a" />
      <rect x="150" y="272" width="112" height="8" rx="4" fill="#31405a" />

      {/* Tablet */}
      <rect x="410" y="118" width="120" height="168" rx="16" fill="#0d1420" stroke="#31405a" strokeWidth="2" />
      {tile(426, 136, 88, 108, "#1b2634")}
      <circle cx="486" cy="160" r="10" fill="#f2a13d" opacity="0.95" />
      <path d="M426 244 L458 196 L480 224 L500 200 L514 214 L514 244 Z" fill="#3ec7c0" opacity="0.8" />
      <circle cx="470" cy="270" r="4" fill="#31405a" />

      {/* Wi-Fi / broadcast arcs (top-right of TV) */}
      <g stroke="#f2a13d" strokeWidth="2.5" fill="none" strokeLinecap="round" opacity="0.9">
        <path d="M356 44 a16 16 0 0 1 24 0" />
        <path d="M362 52 a9 9 0 0 1 12 0" />
        <circle cx="368" cy="60" r="1.6" fill="#f2a13d" stroke="none" />
      </g>
    </svg>
  );
}
