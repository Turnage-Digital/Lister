import * as React from "react";

import { QueryClient } from "@tanstack/react-query";
import { Navigate, createBrowserRouter, redirect } from "react-router-dom";

import { getStoredAuth } from "./auth";
import {
  CreateListItemPage,
  CreateListPage,
  ListItemDetailsPage,
  ListItemsPage,
  ListsPage,
  ResetPasswordPage,
  SignInPage,
  SignUpPage,
  getListSearch,
} from "./pages";
import {
  itemQueryOptions,
  listItemDefinitionQueryOptions,
  listNamesQueryOptions,
  pagedItemsQueryOptions,
} from "./query-options";
import Shell from "./shell";

export const createAppRouter = (queryClient: QueryClient) =>
  createBrowserRouter([
    {
      path: "/",
      element: <Shell />,
      loader: ({ request }) => {
        const auth = getStoredAuth();
        if (auth.status !== "loggedIn") {
          const url = new URL(request.url);
          const callbackUrl = `${url.pathname}${url.search}${url.hash}`;
          const search = callbackUrl
            ? `?callbackUrl=${encodeURIComponent(callbackUrl)}`
            : "";

          throw redirect(`/sign-in${search}`);
        }
        return null;
      },
      children: [
        {
          index: true,
          loader: async () => {
            await queryClient.ensureQueryData(listNamesQueryOptions());
            return null;
          },
          element: <ListsPage />,
        },
        {
          path: "create",
          element: <CreateListPage />,
        },
        {
          path: ":listId",
          loader: async ({ params }) => {
            const listId = params.listId;
            if (!listId) {
              throw new Response("Not Found", { status: 404 });
            }
            await queryClient.ensureQueryData(
              listItemDefinitionQueryOptions(listId),
            );
            return null;
          },
          children: [
            {
              index: true,
              loader: async ({ request, params }) => {
                const listId = params.listId;
                if (!listId) {
                  throw new Response("Not Found", { status: 404 });
                }
                const searchParams = new URL(request.url).searchParams;
                const search = getListSearch(searchParams);
                await queryClient.ensureQueryData(
                  pagedItemsQueryOptions(search, listId),
                );
                return null;
              },
              element: <ListItemsPage />,
            },
            {
              path: "create",
              element: <CreateListItemPage />,
            },
            {
              path: ":itemId",
              loader: async ({ params }) => {
                const listId = params.listId;
                const itemId = params.itemId;
                if (!listId || !itemId) {
                  throw new Response("Not Found", { status: 404 });
                }
                await queryClient.ensureQueryData(
                  itemQueryOptions(listId, Number(itemId)),
                );
                return null;
              },
              children: [
                {
                  index: true,
                  element: <ListItemDetailsPage />,
                },
              ],
            },
          ],
        },
      ],
    },
    {
      path: "/sign-in",
      element: <SignInPage />,
    },
    {
      path: "/sign-up",
      element: <SignUpPage />,
    },
    {
      path: "/reset-password",
      element: <ResetPasswordPage />,
    },
    {
      path: "*",
      element: <Navigate to="/" replace />,
    },
  ]);
