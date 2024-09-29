import React from "react";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_auth/create")({
  component: () => <div>Hello /_auth/create!</div>,
});
