import React from "react";

import { createRoot } from "react-dom/client";

import { AccountMenu } from "./account-menu";

const accountMenuRoot = document.getElementById("account-menu");
if (accountMenuRoot) {
  const root = createRoot(accountMenuRoot);
  root.render(<AccountMenu />);
}
