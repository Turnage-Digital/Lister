import { Card, CardHeader } from "@mui/material";
import React from "react";

import { Item } from "../api";

export interface ItemCardProps {
  item: Item;
}

const ItemCard = ({ item }: ItemCardProps) => {
  return (
    <Card variant="outlined">
      <CardHeader title={`Item # ${item.id}`} />
    </Card>
  );
};

export default ItemCard;
