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
    <div style={{ minHeight: "100vh", display: "grid", placeItems: "center", background: "#eceff3", fontFamily: "system-ui, sans-serif" }}>
      <form onSubmit={submit} style={{ width: 340, background: "#fff", border: "1px solid #dde1e8", borderRadius: 14, padding: 28 }}>
        <div style={{ fontWeight: 800, fontSize: 22, marginBottom: 4 }}>
          Retailer<span style={{ color: "#d1770a" }}>Display</span>
        </div>
        <p style={{ marginTop: 0, color: "#616b7a", fontSize: 14 }}>Sign in to your account</p>
        <div style={{ display: "flex", flexDirection: "column", gap: 12, marginTop: 12 }}>
          <TextInput value={email} onChange={setEmail} placeholder="Email" type="email" />
          <TextInput value={password} onChange={setPassword} placeholder="Password" type="password" />
          <Button type="submit" disabled={busy}>{busy ? "Signing in…" : "Sign in"}</Button>
          <ErrorText>{error}</ErrorText>
        </div>
      </form>
    </div>
  );
}
