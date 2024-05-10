import type { Meta, StoryObj } from "@storybook/react";

import { ItemCard } from "../components";

const meta = {
  title: "ItemCard",
  component: ItemCard,
} satisfies Meta<typeof ItemCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    item: {
      id: "1",
      listId: "1",
      bag: {
        city: "Spring Hill",
        name: "Ricky Lafluer",
        state: "FL",
        status: "Active",
        address: "The Sh*tmobile",
        zipCode: "34609",
        dateOfBirth: "1970-01-01T05:00:00.0000000Z",
      },
    },
  },
};
