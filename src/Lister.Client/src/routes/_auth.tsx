import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_auth")({
  beforeLoad: ({ context, location }) => {
    if (context.auth.status === "loggedOut") {
      throw redirect({
        to: "/sign-in",
        search: {
          callbackUrl: location.href,
        },
      });
    }
  },
});
