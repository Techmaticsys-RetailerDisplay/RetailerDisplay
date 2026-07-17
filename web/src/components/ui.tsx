import type { ReactNode } from "react";

export function PageHeader({ title, subtitle, action }: { title: string; subtitle?: string; action?: ReactNode }) {
  return (
    <div className="page-header">
      <div>
        <h1 className="page-title">{title}</h1>
        {subtitle && <div className="page-subtitle">{subtitle}</div>}
      </div>
      {action}
    </div>
  );
}

export function Card({ children, pad = true, className = "" }: { children: ReactNode; pad?: boolean; className?: string }) {
  return <div className={`card ${pad ? "card-pad" : ""} ${className}`}>{children}</div>;
}

export function StatCard({ label, value, icon, tone }: { label: string; value: ReactNode; icon?: ReactNode; tone?: "good" | "danger" }) {
  return (
    <div className="card stat-card">
      <div className="stat-top">
        <div className="stat-value">{value}</div>
        {icon && <div className={`stat-icon ${tone ?? ""}`}>{icon}</div>}
      </div>
      <div className="stat-label">{label}</div>
    </div>
  );
}

type BtnProps = {
  children: ReactNode; onClick?: () => void; type?: "button" | "submit";
  variant?: "primary" | "ghost" | "danger"; disabled?: boolean; size?: "sm" | "md";
};
export function Button({ children, onClick, type = "button", variant = "primary", disabled, size = "md" }: BtnProps) {
  return (
    <button type={type} onClick={onClick} disabled={disabled}
      className={`btn btn-${variant} ${size === "sm" ? "btn-sm" : ""}`}>
      {children}
    </button>
  );
}

export function TextInput({ value, onChange, placeholder, type = "text" }: {
  value: string; onChange: (v: string) => void; placeholder?: string; type?: string;
}) {
  return (
    <input className="input" value={value} onChange={(e) => onChange(e.target.value)} placeholder={placeholder} type={type} />
  );
}

export function StatusPill({ status }: { status: string }) {
  const online = status.toLowerCase() === "online" || status.toLowerCase() === "active";
  return <span className={`pill ${online ? "pill-online" : "pill-offline"}`}>{status}</span>;
}

export function ErrorText({ children }: { children: ReactNode }) {
  return children ? <div className="error-text">{children}</div> : null;
}

export function EmptyRow({ colSpan, children }: { colSpan: number; children: ReactNode }) {
  return <tr><td colSpan={colSpan}><div className="empty">{children}</div></td></tr>;
}
