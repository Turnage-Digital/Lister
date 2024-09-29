import React from "react";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_auth/$listId/$itemId")({
  component: () => <div>Hello /_auth/$listId/items/$itemId!</div>,
});
