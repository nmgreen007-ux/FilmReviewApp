import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import type { Configuration } from "@azure/msal-browser";
import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";

const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_MSAL_CLIENT_ID,
    authority: import.meta.env.VITE_MSAL_AUTHORITY,
    redirectUri: import.meta.env.VITE_REDIRECT_URI,
  },
};

const msalInstance = new PublicClientApplication(msalConfig);

msalInstance.initialize().then(() => {
  msalInstance.handleRedirectPromise().then((result) => {
    if (result) {
      msalInstance.setActiveAccount(result.account);
    }
    createRoot(document.getElementById("root")!).render(
      <MsalProvider instance={msalInstance}>
        <App />
      </MsalProvider>,
    );
  });
});
