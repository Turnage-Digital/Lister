import React, {
  createContext,
  PropsWithChildren,
  ReactNode,
  useContext,
  useMemo,
  useState,
} from "react";

interface SideDrawerValue {
  openDrawer: (title: string, content: ReactNode) => void;
  closeDrawer: () => void;
  title: string;
  content: ReactNode;
}

const defaultValue: SideDrawerValue = {
  openDrawer: () => {},
  closeDrawer: () => {},
  title: "",
  content: null,
};

const SideDrawerContext = createContext<SideDrawerValue>(defaultValue);

const SideDrawerProvider = ({ children }: PropsWithChildren) => {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState<ReactNode>(null);

  const openDrawer = (title: string, content: ReactNode): void => {
    setTitle(title);
    setContent(content);
  };

  const closeDrawer = (): void => {
    setContent(null);
  };

  const value = useMemo(
    () => ({
      openDrawer,
      closeDrawer,
      title,
      content,
    }),
    [content, title]
  );

  return (
    <SideDrawerContext.Provider value={value}>
      {children}
    </SideDrawerContext.Provider>
  );
};

export default SideDrawerProvider;

export function useSideDrawer() {
  return useContext(SideDrawerContext);
}
