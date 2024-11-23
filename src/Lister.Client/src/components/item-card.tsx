import * as React from "react";

import { Card, CardHeader } from "@mui/material";

import { ListItem } from "../models";

export interface Props {
  item: ListItem;
}

const ItemCard = ({ item }: Props) => {
  return (
    <Card>
      <CardHeader title={`Item # ${item.id}`} />
    </Card>
  );
};

export default ItemCard;
