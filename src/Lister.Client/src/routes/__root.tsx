import * as React from "react";

import { QueryClient } from "@tanstack/react-query";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";

import { Auth } from "../auth";
import { AppLayout, SideDrawer } from "../components";

const RootComponent = () => {
  const { auth, status } = Route.useRouteContext({
    select: ({ auth }) => ({ auth, status: auth.status }),
  });

  return (
    <>
      <AppLayout auth={auth} status={status}>
        <Outlet />
      </AppLayout>

      <SideDrawer />
    </>
  );
};

export const Route = createRootRouteWithContext<{
  auth: Auth;
  queryClient: QueryClient;
}>()({
  component: RootComponent,
});
