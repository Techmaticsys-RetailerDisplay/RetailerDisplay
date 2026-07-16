import type { ReactNode, CSSProperties } from "react";

export function PageHeader({ title, subtitle, action }: { title: string; subtitle?: string; action?: ReactNode }) {
  return (
    <div style={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between", marginBottom: 24 }}>
      <div>
        <h1 style={{ margin: "0 0 4px", fontSize: 26 }}>{title}</h1>
        {subtitle && <p style={{ margin: 0, color: "#616b7a", fontSize: 14 }}>{subtitle}</p>}
      </div>
      {action}
    </div>
  );
}

export function Card({ children, style }: { children: ReactNode; style?: CSSProperties }) {
  return (
    <div style={{ background: "#fff", border: "1px solid #dde1e8", borderRadius: 12, padding: 20, ...style }}>
      {children}
    </div>
  );
}

export function StatCard({ label, value, accent }: { label: string; value: ReactNode; accent?: string }) {
  return (
    <Card style={{ minWidth: 150 }}>
      <div style={{ fontSize: 28, fontWeight: 800, color: accent ?? "#10151f" }}>{value}</div>
      <div style={{ fontSize: 13, color: "#616b7a", marginTop: 2 }}>{label}</div>
    </Card>
  );
}

export function Button({ children, onClick, type = "button", variant = "primary", disabled }: {
  children: ReactNode; onClick?: () => void; type?: "button" | "submit"; variant?: "primary" | "ghost" | "danger"; disabled?: boolean;
}) {
  const styles: Record<string, CSSProperties> = {
    primary: { background: "#d1770a", color: "#fff", border: "none" },
    ghost: { background: "transparent", color: "#2b3444", border: "1px solid #c4cbd6" },
    danger: { background: "transparent", color: "#d13a3a", border: "1px solid #e6b0b0" },
  };
  return (
    <button type={type} onClick={onClick} disabled={disabled}
      style={{ padding: "8px 14px", borderRadius: 8, fontSize: 14, fontWeight: 600, cursor: disabled ? "default" : "pointer", opacity: disabled ? 0.6 : 1, ...styles[variant] }}>
      {children}
    </button>
  );
}

export function TextInput({ value, onChange, placeholder, type = "text" }: {
  value: string; onChange: (v: string) => void; placeholder?: string; type?: string;
}) {
  return (
    <input value={value} onChange={(e) => onChange(e.target.value)} placeholder={placeholder} type={type}
      style={{ padding: "9px 12px", borderRadius: 8, border: "1px solid #c4cbd6", fontSize: 14, width: "100%", boxSizing: "border-box" }} />
  );
}

export function ErrorText({ children }: { children: ReactNode }) {
  return children ? <div style={{ color: "#d13a3a", fontSize: 13, marginTop: 8 }}>{children}</div> : null;
}

export function StatusPill({ status }: { status: string }) {
  const online = status.toLowerCase() === "online";
  return (
    <span style={{
      fontSize: 12, fontWeight: 700, padding: "2px 9px", borderRadius: 999,
      background: online ? "#dcf3e6" : "#f0f0f2", color: online ? "#1f9d57" : "#8a94a3",
    }}>
      {status}
    </span>
  );
}
