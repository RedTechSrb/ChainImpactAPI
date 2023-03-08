import {
  Avatar,
  Badge,
  createStyles,
  Group,
  Modal,
  ScrollArea,
  Table,
  Text,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { Link } from "react-router-dom";
import { useGetImpactorsWithDonations } from "../../repositories/ImpactorRepository";

interface UsersTableProps {
  title: string;
  data: {
    avatar: string;
    name: string;
    job: string;
    wallet: string;
    role: string;
    amount: number;
  }[];
  titlecolor: string;
  type: string;
  isPrivate: boolean;
}

const rolesData = ["Manager", "Collaborator", "Contractor"];

const useStyles = createStyles((theme) => ({
  badge: {
    textAlign: "center",
    color: "titlecolor",
    cursor: "pointer",
  },
}));

export default function ImpactorTable({
  data,
  title,
  titlecolor,
  type,
  isPrivate,
}: UsersTableProps) {
  function arangeData(data: any, arangeFromAPI: boolean) {
    let impactorData = data;
    if (arangeFromAPI) {
      impactorData = data.map((impactor: any) => ({
        avatar: impactor.imageurl
          ? impactor.imageurl
          : "https://avatars.githubusercontent.com/u/1309537?v=4",
        name: impactor.name,
        job: "",
        wallet: impactor.wallet,
        role: "Company",
        amount: impactor.totalDonations,
      }));
    }

    const rows = impactorData.map((item: any) => (
      <tr key={item.name}>
        <td>
          <Link
            to={`/company/${item.wallet}`}
            style={{ textDecoration: "none" }}
          >
            <Group spacing="sm">
              <Avatar size={40} src={item.avatar} radius={40} />
              <div>
                <Text size="sm" weight={500}>
                  {item.name}
                </Text>
                <Text size="xs" color="dimmed">
                  {item.wallet}
                </Text>
              </div>
            </Group>
          </Link>
        </td>
        <td>{item.amount}</td>
      </tr>
    ));

    return rows;
  }

  const dataAll = useGetImpactorsWithDonations(
    {
      pageNumber: null,
      pageSize: null,
      dto: {
        projectType: type,
      },
    },
    isPrivate
  );

  const { classes } = useStyles();
  const [opened, { open, close }] = useDisclosure(false);

  const rows = arangeData(data, false);
  const rowsModal = arangeData(dataAll, true);

  return (
    <div>
      <Modal
        opened={opened}
        onClose={close}
        title={"Top Impactors in " + type.toUpperCase() + " category"}
      >
        <ScrollArea>
          <Table sx={{ minWidth: 400 }} verticalSpacing="sm">
            <thead>
              <tr>
                <th>{isPrivate ? "Impactor" : "Company"}</th>
                <th>Amount</th>
              </tr>
            </thead>
            <tbody>{rowsModal}</tbody>
          </Table>
        </ScrollArea>
      </Modal>

      <Badge
        className={classes.badge}
        style={{ color: titlecolor }}
        size="lg"
        onClick={open}
      >
        {title}
      </Badge>
      <ScrollArea>
        <Table sx={{ minWidth: 400 }} verticalSpacing="sm">
          <thead>
            <tr>
              <th>{isPrivate ? "Impactor" : "Company"}</th>
              <th>Amount</th>
            </tr>
          </thead>
          <tbody>{rows}</tbody>
        </Table>
      </ScrollArea>
    </div>
  );
}
