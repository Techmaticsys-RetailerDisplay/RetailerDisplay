import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { authApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import { useAuth } from "../../auth/AuthContext";
import type { UpdateProfileRequest } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText } from "../../components/ui";

const EMPTY: UpdateProfileRequest = {
  businessName: "", contactName: "", phone: "",
  addressLine1: "", addressLine2: "", city: "", state: "", postalCode: "", country: "",
};

export default function ProfilePage() {
  const { retailer, refreshProfile } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState<UpdateProfileRequest>(EMPTY);
  const [error, setError] = useState<string | null>(null);
  const [note, setNote] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const incomplete = retailer && !retailer.profileCompleted;

  useEffect(() => {
    authApi.getProfile().then((p) =>
      setForm({
        businessName: p.businessName ?? "", contactName: p.contactName ?? "", phone: p.phone ?? "",
        addressLine1: p.addressLine1 ?? "", addressLine2: p.addressLine2 ?? "", city: p.city ?? "",
        state: p.state ?? "", postalCode: p.postalCode ?? "", country: p.country ?? "",
      })
    ).catch((e) => setError(apiError(e)));
  }, []);

  function set<K extends keyof UpdateProfileRequest>(key: K, value: string) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function save() {
    setError(null); setNote(null); setBusy(true);
    try {
      const updated = await authApi.updateProfile(form);
      await refreshProfile();
      if (updated.profileCompleted) {
        navigate("/");
      } else {
        setNote("Saved. Please fill all required fields (marked *) to finish.");
      }
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <div>
      <PageHeader title="Profile" subtitle="Your business and contact details" />
      {incomplete && (
        <div className="card card-pad" style={{ marginBottom: 16, borderLeft: "3px solid var(--brand)" }}>
          Welcome! Please complete your profile before using the rest of the app.
        </div>
      )}
      <Card>
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14, maxWidth: 640 }}>
          <Field label="Business name *"><TextInput value={form.businessName} onChange={(v) => set("businessName", v)} /></Field>
          <Field label="Email"><input className="input" value={retailer?.email ?? ""} disabled /></Field>
          <Field label="Contact name *"><TextInput value={form.contactName ?? ""} onChange={(v) => set("contactName", v)} /></Field>
          <Field label="Phone *"><TextInput value={form.phone ?? ""} onChange={(v) => set("phone", v)} /></Field>
          <Field label="Address line 1 *"><TextInput value={form.addressLine1 ?? ""} onChange={(v) => set("addressLine1", v)} /></Field>
          <Field label="Address line 2"><TextInput value={form.addressLine2 ?? ""} onChange={(v) => set("addressLine2", v)} /></Field>
          <Field label="City *"><TextInput value={form.city ?? ""} onChange={(v) => set("city", v)} /></Field>
          <Field label="State *"><TextInput value={form.state ?? ""} onChange={(v) => set("state", v)} /></Field>
          <Field label="Postal code *"><TextInput value={form.postalCode ?? ""} onChange={(v) => set("postalCode", v)} /></Field>
          <Field label="Country *"><TextInput value={form.country ?? ""} onChange={(v) => set("country", v)} /></Field>
        </div>
        <div style={{ marginTop: 18 }}>
          <Button onClick={save} disabled={busy}>{busy ? "Saving…" : "Save profile"}</Button>
        </div>
        <ErrorText>{error}</ErrorText>
        {note && <div className="ok-text">{note}</div>}
      </Card>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div>
      <label className="field-label">{label}</label>
      {children}
    </div>
  );
}
