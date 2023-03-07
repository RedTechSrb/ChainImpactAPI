import {
  Card,
  Image,
  Text,
  Group,
  Badge,
  createStyles,
  Center,
  Button,
  Progress,
  TextInput,
} from "@mantine/core";
import { Icon123 } from "@tabler/icons";
import { Project } from "../../models/Project";
import { ProgressProject } from "../ProgressProject";

type DonationSidebarProps = {
  project: Project;
  sidebarTop: number;
};

const useStyles = createStyles((theme) => ({
  card: {
    backgroundColor:
      theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
  },

  imageSection: {
    padding: theme.spacing.md,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    borderBottom: `${1}} solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[3]
    }`,
  },

  label: {
    marginBottom: theme.spacing.xs,
    lineHeight: 1,
    fontWeight: 700,
    fontSize: theme.fontSizes.xs,
    letterSpacing: "-0.25",
    textTransform: "uppercase",
  },

  section: {
    padding: theme.spacing.md,
    borderTop: `${1} solid ${
      theme.colorScheme === "dark" ? theme.colors.dark[4] : theme.colors.gray[3]
    }`,
  },

  image: {
    maxHeight: "240px",
    maxWidth: "320px",
  },

  icon: {
    marginRight: "5px",
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[2]
        : theme.colors.gray[5],
  },
}));

const mockdata = [
  { label: "4 passengers" },
  { label: "100 km/h in 4 seconds" },
  { label: "Automatic gearbox" },
  { label: "Electric" },
];

export default function DonationSidebar({
  project,
  sidebarTop,
}: DonationSidebarProps) {
  const { classes } = useStyles();
  const features = mockdata.map((feature) => (
    <Center key={feature.label}>
      <Icon123 size="1.05rem" className={classes.icon} stroke={1.5} />
      <Text size="xs">{feature.label}</Text>
    </Center>
  ));

  return (
    <Card
      withBorder
      radius="md"
      className={classes.card}
      style={{ top: `${sidebarTop}px` }}
    >
      <Card.Section className={classes.imageSection}>
        <Image
          src="https://media.istockphoto.com/id/174062115/photo/homeless-people.jpg?s=612x612&w=is&k=20&c=9fbaYUH1LNfNUsPopf1lwKjtSDwdYLb2lENKvZCVPWA="
          alt="Tesla Model S"
          className={classes.image}
        />
      </Card.Section>

      <Text size={"xl"} weight={500} mt="lg" color={"#BBFD00"}>
        {project.name}
      </Text>
      <ProgressProject
        projectData={project}
        mtVal={"lg"}
        mbVal={"0"}
      ></ProgressProject>

      <Card.Section className={classes.section}>
        {/* <TextInput
          placeholder="Amount in USDC"
          label="Amount"
          //error="Wallet not connected, please press connect wallet"
          radius="lg"
          withAsterisk
        /> */}
        <Button radius="sm" style={{ flex: 1, width: "100%" }} mt="xl">
          Donate
        </Button>
      </Card.Section>
    </Card>
  );
}
