//Automation of tests while user is logged into SyllabusZip

describe('Logged in User Tests', () =>{
    beforeEach(() => {
        cy.visit('https://syllabuszip.com')
        cy.Login()
    })
    afterEach(() => {
        cy.Logout()
    })

    //TC-27: ClickingCourse_ClickingOverview_LoadsCourseOverview
    it('Navigates to Course Overview', () => {

        cy.get('a[href*="/Home/Course"]').contains('ACCT').click()
        cy.get('.dropdown-item').contains('Overview').click()

        cy.url().should('include', '/Home/Course')
    })

    //TC-28: ClickingCourse_ClickingMaterials_LoadsCourseMaterials
    it('Navigates to Course Materials', () => {
        cy.get('a[href*="/Home/Course"]').contains('ACCT').click()
        cy.get('.dropdown-item').contains('Materials').click()

        cy.url().should('include', '/Home/Materials')
    })

    //TC-29: ClickingCourse_ClickingCalendar_LoadsCourseCalendar
    it('Navigates to Course Calendar', () => {
        cy.get('a[href*="/Home/Course"]').contains('ACCT').click()
        cy.get('.dropdown-item').contains('Calendar').click()

        cy.url().should('include', '/Home/Calendar')
    })

    //TC-30: ClickingCourse_ClickingAssignments_LoadsCourseAssignments
    it('Navigates to Assignments', () => {
        cy.get('a[href*="/Home/Course"]').contains('ACCT').click()
        cy.get('.dropdown-item').contains('Assignments').click()

        cy.url().should('include', '/Home/Assignments')
    }) 

    //TC-31: ClickingCourse_ClickingExams_LoadsCourseExams
    it('Navigates to Exams', () => {
        cy.get('a[href*="/Home/Course"]').contains('ACCT').click()
        cy.get('.dropdown-item').contains('Exams').click()

        cy.url().should('include', '/Home/Exams')

    })
})