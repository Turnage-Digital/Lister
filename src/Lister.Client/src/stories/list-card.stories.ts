import type { Meta, StoryObj } from "@storybook/react";

import { ListCard } from "../components";

const meta = {
  title: "ListCard",
  component: ListCard,
} satisfies Meta<typeof ListCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    listName: {
      id: "1",
      name: "List Name",
    },
    onViewClick: () => {},
  },
};
