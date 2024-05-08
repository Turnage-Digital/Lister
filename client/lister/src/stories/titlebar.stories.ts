import type { Meta, StoryObj } from "@storybook/react";

import { Action, Titlebar } from "../components";

const meta = {
  title: "Titlebar",
  component: Titlebar,
} satisfies Meta<typeof Titlebar>;

export default meta;
type Story = StoryObj<typeof meta>;

export const ListsPage: Story = {
  args: {
    title: "Lists",
    actions: [
      {
        title: "Create a List",
      },
    ],
  },
};

export const ListPage: Story = {
  args: {
    title: "Trailer Park Boys",
    actions: [
      {
        title: "Create an Item",
      },
    ],
    breadcrumbs: [
      {
        title: "Lists",
        url: "/",
      },
    ],
  },
};

export const ItemPage: Story = {
  args: {
    title: "Item #1",
    breadcrumbs: [
      {
        title: "Lists",
        url: "/",
      },
      {
        title: "Trailer Park Boys",
        url: "/08dc6e3a-9704-42f2-800c-4231f00ea3b1",
      },
    ],
  },
};
