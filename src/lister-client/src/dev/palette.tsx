import MUIPalette from "@react-buddy/palette-mui";
import React from "react";
import {
  Category,
  Component,
  Palette,
  Variant,
} from "@react-buddy/ide-toolbox";

import {
  FormBlock,
  Loading,
  StatusBullet,
  StatusChip,
  Titlebar,
} from "../components";
import { statusColors } from "../api";

export const PaletteTree = () => (
  <Palette>
    <Category name="App">
      <Component name="FormBlock">
        <Variant>
          <FormBlock
            title="Test"
            blurb="Test blurb goes here."
            content={<div>Hello World!</div>}
          />
        </Variant>
      </Component>
      <Component name="Loading">
        <Variant>
          <Loading />
        </Variant>
      </Component>
      <Component name="StatusBullet">
        <Variant>
          <StatusBullet statusColor={statusColors[0]} />
        </Variant>
      </Component>
      <Component name="StatusChip">
        <Variant>
          <StatusChip />
        </Variant>
      </Component>
      <Component name="Titlebar">
        <Variant>
          <Titlebar title="Lists" />
        </Variant>
      </Component>
    </Category>
    <MUIPalette />
  </Palette>
);
