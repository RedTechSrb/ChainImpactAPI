import {
  ActionIcon,
  Container,
  createStyles,
  Grid,
  Group,
  Loader,
  SimpleGrid,
  Text,
  Title,
  useMantineTheme,
} from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import {
  IconBrandDiscord,
  IconBrandInstagram,
  IconBrandTwitter,
} from "@tabler/icons";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import AngelImpactor from "../components/projectComponents/AngelImpactor";
import DonationSidebar from "../components/projectComponents/DonationSidebar";
import RecentImpactors from "../components/projectComponents/RecentImpactors";
import { Project } from "../models/Project";
import { useGetSpecificProject } from "../repositories/ProjectRepository";
import NotFound from "./NotFound";

const PRIMARY_COL_HEIGHT = "32rem";

const useStyles = createStyles((theme) => ({
  container: {
    marginLeft: "5rem",
    marginRight: "5rem",
    marginTop: "2.5rem",
    [theme.breakpoints.sm]: {
      marginLeft: "2.5rem",
      marginRight: "2.5rem",
    },
  },
  links: {
    [theme.fn.smallerThan("xs")]: {
      marginTop: theme.spacing.md,
    },
  },
  grid: {
    maxHeight: "60vh",
  },
  loadingContainer: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "623px",
  },
  loadingBar: {
    margin: "15vh auto 10px auto",
    fontSize: "30px",
  },
}));

export default function ProjectOverview() {
  const [isLoading, setIsLoading] = useState(true);
  const [sidebarTop, setSidebarTop] = useState(0);

  const mobile = useMediaQuery(`(max-width: 900px)`);
  let { id } = useParams();
  const projectSearch = { dto: { id: Number(id) } };
  const projectData: Project | undefined = useGetSpecificProject(projectSearch);

  useEffect(() => {
    if (projectData) setIsLoading(false);

    const timeoutId = setTimeout(() => {
      setIsLoading(false);
    }, 3000);

    return () => {
      clearTimeout(timeoutId);
    };
  }, [isLoading, projectData]);

  const angelimpactor = {
    imageurl: "https://picsum.photos/id/1/200",
    name: "John Doe",
    wallet: "0x1234567890abcdef",
  };

  const { classes } = useStyles();
  const laptop = useMediaQuery(`(max-width: 1440px)`);
  useEffect(() => {
    function handleScroll() {
      setSidebarTop(window.scrollY);
    }
    window.addEventListener("scroll", handleScroll);
    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);
  return (
    <>
      <Container size={1750}>
        {isLoading ? (
          <Container size={1750} className={classes.loadingContainer}>
            <Text className={classes.loadingBar}>Loading project data</Text>
            <Loader variant="dots" />
          </Container>
        ) : projectData ? (
          <Grid className={classes.container}>
            <Grid.Col span={mobile ? 12 : 9}>
              {/* <Image
                src="https://media.istockphoto.com/id/506664332/photo/business-with-csr-practice.jpg?s=1024x1024&w=is&k=20&c=qKTzGl0Wec-oxJ_sU-eTcPDzooTSqIyHIh3rmIeUNcI="
                alt="alt"
                height={180}
                mb="md"
              /> */}
              <SimpleGrid cols={2} verticalSpacing="sm">
                <Group>
                  <Title>{projectData?.name}</Title>
                </Group>
                <Group
                  spacing={0}
                  className={classes.links}
                  position="right"
                  noWrap
                >
                  <Text size="md" weight={500} color="white">
                    Connect on socials
                  </Text>
                  <ActionIcon
                    size="lg"
                    component="a"
                    href={projectData?.twitter ?? undefined}
                  >
                    <IconBrandTwitter size="1.05rem" stroke={1.5} />
                  </ActionIcon>
                  <ActionIcon
                    size="lg"
                    component="a"
                    href={projectData?.discord ?? undefined}
                  >
                    <IconBrandDiscord size="1.05rem" stroke={1.5} />
                  </ActionIcon>
                  <ActionIcon
                    size="lg"
                    component="a"
                    href={projectData?.instagram ?? undefined}
                  >
                    <IconBrandInstagram size="1.05rem" stroke={1.5} />
                  </ActionIcon>
                </Group>
              </SimpleGrid>

              <Text size={24} weight={500} color="white" mt="sm">
                Description:
              </Text>
              <Text size="md" color="white" mb="xl">
                {projectData?.description}
              </Text>

              <SimpleGrid
                cols={2}
                verticalSpacing="sm"
                mb="xl"
                className={classes.grid}
              >
                <Text
                  size={laptop ? 20 : 24}
                  weight={500}
                  color="white"
                  mt="sm"
                >
                  Recent Impactors:
                </Text>
                <Text
                  size={laptop ? 20 : 24}
                  weight={500}
                  color="white"
                  mt="sm"
                >
                  Angel Impactor who brought this project to life.
                </Text>

                <RecentImpactors></RecentImpactors>

                <AngelImpactor
                  impactor={projectData.angelimpactor}
                  totalbacked={0}
                  totaldonated={0}
                ></AngelImpactor>
              </SimpleGrid>

              <Text size="xl" weight={500} mb="xl">
                Biggest donators
              </Text>
              <RecentImpactors></RecentImpactors>

              <Text size="xl" weight={500} mb="xl">
                Milestones
              </Text>
              <RecentImpactors></RecentImpactors>
            </Grid.Col>

            <Grid.Col span={mobile ? 12 : 3}>
              <DonationSidebar
                project={projectData}
                sidebarTop={sidebarTop}
              ></DonationSidebar>
            </Grid.Col>
          </Grid>
        ) : (
          <NotFound />
        )}
      </Container>
    </>
  );
}
