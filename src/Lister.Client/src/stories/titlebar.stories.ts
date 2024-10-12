import type { Meta, StoryObj } from "@storybook/react";

import { Titlebar } from "../components";

const meta = {
  title: "Titlebar",
  component: Titlebar
} satisfies Meta<typeof Titlebar>;

export default meta;
type Story = StoryObj<typeof meta>;

export const ListsPage: Story = {
  args: {
    title: "Lists",
    actions: [
      {
        title: "Create a List"
      }
    ]
  }
};

export const ListPage: Story = {
  args: {
    title: "Students",
    actions: [
      {
        title: "Add an Item"
      }
    ],
    breadcrumbs: [
      {
        title: "Lists"
      }
    ]
  }
};

export const ItemPage: Story = {
  args: {
    title: "ID 6732",
    breadcrumbs: [
      {
        title: "Lists"
      },
      {
        title: "Students"
      }
    ]
  }
};
