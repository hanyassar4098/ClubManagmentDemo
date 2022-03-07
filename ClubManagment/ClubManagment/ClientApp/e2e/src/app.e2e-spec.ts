




import { AppPage } from './app.po';

describe('ClubManagment App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: ClubManagment', async () => {
    await page.navigateTo();
    expect(await page.getAppTitle()).toEqual('ClubManagment');
  });
});
