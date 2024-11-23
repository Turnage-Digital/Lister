import * as React from "react";
import { PropsWithChildren } from "react";

type Props = PropsWithChildren;

const SideDrawerContainer = ({ children }: Props) => {
  return <>{children}</>;
};

export default SideDrawerContainer;
