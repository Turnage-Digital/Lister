import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_auth")({
  beforeLoad: async ({ location }) => {
    const request = new Request("/identity/manage/info", {
      method: "GET",
    });
    const response = await fetch(request);
    if (response.status === 401) {
      throw redirect({
        to: "/sign-in",
        search: {
          callbackUrl: location.href,
        },
      });
    }
  },
});
