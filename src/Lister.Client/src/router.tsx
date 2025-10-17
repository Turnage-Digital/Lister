import * as React from "react";

import { QueryClient } from "@tanstack/react-query";
import { createBrowserRouter, Navigate } from "react-router-dom";

import {
  CreateListItemPage,
  CreateListPage,
  EditListItemPage,
  EditListPage,
  ForgotPasswordPage,
  getListSearch,
  ListItemDetailsPage,
  ListItemsPage,
  ListsPage,
  ResetPasswordPage,
  SignInPage,
  SignUpPage,
} from "./pages";
import {
  itemQueryOptions,
  listItemDefinitionQueryOptions,
  notificationRulesQueryOptions,
  pagedItemsQueryOptions,
} from "./query-options";
import Shell from "./shell";

export const createAppRouter = (queryClient: QueryClient) =>
  createBrowserRouter([
    {
      path: "/",
      element: <Shell />,
      children: [
        {
          index: true,
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
              path: "edit",
              loader: async ({ params }) => {
                const listId = params.listId;
                if (!listId) {
                  throw new Response("Not Found", { status: 404 });
                }
                await queryClient.ensureQueryData(
                  notificationRulesQueryOptions(listId),
                );
                return null;
              },
              element: <EditListPage />,
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
                {
                  path: "edit",
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
                  element: <EditListItemPage />,
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
      path: "/forgot-password",
      element: <ForgotPasswordPage />,
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
